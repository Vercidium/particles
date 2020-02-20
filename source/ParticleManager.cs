using OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Particles
{
    class ParticleManager
    {
        public ParticleManager()
        {
            ModelHelper.InitialiseTrigonometry();
            GetThreadCount();

            map = new Map();
            ParticleVertexBuffer = ModelHelper.CreateParticleBuffer();
            ParticleVertexBuffer.BufferData();
            ParticleColourBuffer = new InstanceBuffer<uint>(ParticleVertexBuffer.arrayHandle, 1024, 1);
            ParticleMatrixBuffer = new InstanceBuffer<Matrix4F>(ParticleVertexBuffer.arrayHandle, 1024, 2);
        }

        Map map;

        public static ParticleVertexBuffer     ParticleVertexBuffer;
        public static InstanceBuffer<uint>     ParticleColourBuffer;
        public static InstanceBuffer<Matrix4F> ParticleMatrixBuffer;

        const int MAX_THREAD_COUNT = 128;
        int THREAD_COUNT;
        int THREAD_COUNT_MINUS_ONE;

        int[] t_ActiveParticleCount = new int[MAX_THREAD_COUNT];

        public Particle[] t_ActiveParticles  = new Particle[MAX_THREAD_COUNT];
        public Particle[] t_DecayedParticles = new Particle[MAX_THREAD_COUNT];
        
        void GetThreadCount()
        {
            ThreadPool.GetMinThreads(out int count, out _);
            THREAD_COUNT = count;
            THREAD_COUNT_MINUS_ONE = count - 1;
        }

        int _ctai;
        public int CurrentThreadAddIndex
        {
            get
            {
                if (_ctai == THREAD_COUNT_MINUS_ONE)
                    _ctai = 0;
                else
                    _ctai++;

                return _ctai;
            }
        }

        int TotalParticleCount()
        {
            int total = 0;
            for (int i = 0; i < THREAD_COUNT; i++)
                total += t_ActiveParticleCount[i];

            return total;
        }
        
        protected void AddParticle(in Vector3 position, in Vector3 velocity, in float scale, in uint colour, in float lifeTime)
        {
            int index = CurrentThreadAddIndex;

            var p = t_DecayedParticles[index].Next;

            // If there are no decayed particles available, allocate a new one
            if (p == null)
            {
                p = new Particle()
                {
                    position = position,
                    velocity = velocity,
                    scale = scale,
                    colour = colour,
                    lifeTime = lifeTime,
                };
            }
            else
            {
                // Modify an existing decayed particle
                p.position = position;
                p.velocity = velocity;
                p.scale = scale;
                p.colour = colour;
                p.lifeTime = lifeTime;

                // Remove the particle from the decayed linked list
                t_DecayedParticles[index].PopNext();

                // Disconnect the particle from the decayed linked list
                p.Next = null;
            }

            // Add the particle to the current active linked list
            t_ActiveParticles[index].AddNext(p);

            // Keep track of how many particles are in this active linked list
            // so that we can allocate the correct buffer size on the GPU
            t_ActiveParticleCount[index]++;
        }

        void UpdateParticles(double elapsed)
        {
            int[] pointerOffsets = new int[THREAD_COUNT];
            int particleCount = 0;

            // Calculate the VBO write offset for each thread
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                pointerOffsets[i] = particleCount;
                particleCount += t_ActiveParticleCount[i];
            }

            if (particleCount == 0)
                return;

            // Check there is enough memory allocated for the VBO
            ParticleMatrixBuffer.Expand(particleCount);
            ParticleColourBuffer.Expand(particleCount);

            // If we only have one thread available, do everything single-threaded without Tasks
            if (THREAD_COUNT == 1)
            {
                // Get a pointer to each instance buffer. Buffer access must be READ_WRITE as that's the attribute
                // returned when calling Gl.GetBufferParameter(...)
                ParticleColourBuffer.Bind();
                var cPtrBase = Gl.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadWrite);
                ParticleMatrixBuffer.Bind();
                var mPtrBase = Gl.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadWrite);

                unsafe
                {
                    // Convert IntPtrs to pointers
                    uint* cPtr = (uint*)cPtrBase.ToPointer();
                    Matrix4F* mPtr = (Matrix4F*)mPtrBase.ToPointer();

                    for (int i = 0; i < THREAD_COUNT; i++)
                    {
                        // If the linked list contains particles
                        if (t_ActiveParticles[i].Next != null)
                            UpdateParticleRange(cPtr, mPtr, i, pointerOffsets[i], elapsed);
                    }
                }

                // Unmap each buffer
                ParticleColourBuffer.Bind();
                Gl.UnmapBuffer(BufferTarget.ArrayBuffer);
                ParticleMatrixBuffer.Bind();
                Gl.UnmapBuffer(BufferTarget.ArrayBuffer);
            }
            else
            {
                var tasks = new List<Task>();
                
                ParticleColourBuffer.Bind();
                var cPtrBase = Gl.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadWrite);
                ParticleMatrixBuffer.Bind();
                var mPtrBase = Gl.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.ReadWrite);

                unsafe
                {
                    uint* cPtr = (uint*)cPtrBase.ToPointer();
                    Matrix4F* mPtr = (Matrix4F*)mPtrBase.ToPointer();

                    for (int i = 0; i < THREAD_COUNT; i++)
                    {
                        if (t_ActiveParticles[i].Next != null)
                        {
                            int index = i;
                            int bufferOffset = pointerOffsets[i];

                            tasks.Add(Task.Run(() => UpdateParticleRange(cPtr, mPtr, index, bufferOffset, elapsed)));
                        }
                    }
                }

                ParticleColourBuffer.Bind();
                Gl.UnmapBuffer(BufferTarget.ArrayBuffer);
                ParticleMatrixBuffer.Bind();
                Gl.UnmapBuffer(BufferTarget.ArrayBuffer);

                // Wait for all particles to be updated
                Task.WaitAll(tasks.ToArray());
            }
        }

        unsafe void UpdateParticleRange(uint* cPtr, Matrix4F* mPtr, int index, int bufferOffset, double elapsed)
        {
            Particle p = t_ActiveParticles[index];
            Particle q = p.Next;
            
            // Precalculate variables
            double thisGravity = Constants.Gravity * elapsed;
            double scaleMultiplier = Math.Pow(0.9, elapsed / 16d);
            float elapsedF = (float)elapsed;
            float elapsed1500 = elapsedF / 1500;

            // Allocate variables
            bool hit = false;
            Axis axis = Axis.None;
            
            // Dereference array accesses
            ref int particleCount = ref t_ActiveParticleCount[index];
            var temporaryParticles = t_DecayedParticles[index];

            while (q != null)
            {
                bool shrinking = false;

                // Particles shrink during the last 1.5s of their lifetime
                if (q.lifeTime < 1500)
                {
                    q.scale -= elapsed1500;

                    // If the particle is virtually invisible, remove it
                    if (q.scale < 0.03)
                    {
                        mPtr[bufferOffset++] = Matrix4F.Hidden;

                        // Remove the particle from the active linked list
                        p.Next = q.Next;
                        q.Next = null;

                        // Add the particle to temporary storage
                        temporaryParticles.AddNext(q);
                        particleCount--;

                        // Get the next active particle
                        q = p.Next;
                        continue;
                    }

                    shrinking = true;
                }

                var elapsedVelocity = q.velocity * elapsed;
                var mag = elapsedVelocity.Magnitude;

                // Only raymarch if the particle is moving
                if (mag > 0.000001)
                    map.RayMarch(q.position, elapsedVelocity, mag, ref hit, ref axis);
                else
                    hit = false;

                if (hit)
                {
                    // Reflect and dampen the particle's velocity based on which axis it collided on
                    q.velocity *= Constants.AxisToBounce[(int)axis];
                }
                else if (!map.NoBlock((int)q.position.X, (int)q.position.Y, (int)q.position.Z))
                {
                    // Slide to a stop along the ground
                    q.velocity.X *= Constants.ParticleSlideDampening;
                    q.velocity.Z *= Constants.ParticleSlideDampening;
                }
                else
                {
                    q.velocity.Y += thisGravity;
                }

                // Recalculate elapsedVelocity
                elapsedVelocity = q.velocity * elapsed;

                q.rotation -= elapsedVelocity;
                q.lifeTime -= elapsedF;

                // Only update the transform if the particle is shrinking or moving
                if (shrinking || elapsedVelocity.Magnitude > 0.000001)
                {
                    q.position += elapsedVelocity;
                    q.transform = ModelHelper.ParticleMatrix(q.scale, (float)q.rotation.X, (float)q.rotation.Z, q.position);
                }

                // Write the colour and transformation matrix directly to shared memory
                cPtr[bufferOffset] = q.colour;
                mPtr[bufferOffset++] = q.transform;

                // Advance the linked list
                q = q.Next;
                p = p.Next;
            }
        }

        protected void RenderParticles()
        {
            // Each particle VBO and instance VBO shares the same VAO
            // Bind the VAO
            Gl.BindVertexArray(ParticleMatrixBuffer.arrayHandle);

            // Draw the particles
            Gl.DrawArraysInstanced(PrimitiveType.Triangles, 0, 36, TotalParticleCount());
            
            // Unbind the VAO
            Gl.BindVertexArray(0);
        }
    }
}
