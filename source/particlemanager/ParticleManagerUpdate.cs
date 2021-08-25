using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Particles
{
    public partial class ParticleManager
    {
        public void UpdateParticles(float elapsed)
        {
            // Determine which particle buffer to update this frame
            var updateIndex = ParticleUpdateIndex;


            // Store the current amount of particles this frame,
            // so we know how many particles to render 2 frames down the line
            int particleCount = ParticleCounts[updateIndex] = TotalParticleCount();


            // If there are particles to update
            if (particleCount > 0)
            {
                Console.WriteLine($"Updating {particleCount} particles");

                // Resize buffers if necessary
                ParticleMatrixBuffer[updateIndex].Expand(particleCount);
                ParticleColourBuffer[updateIndex].Expand(particleCount);


                // Update particles and copy their colour+matrix to shared GPU memory
                UpdateParticles(elapsed, updateIndex, particleCount);
            }
        }

        unsafe void UpdateParticles(float elapsed, int updateIndex, int particleCount)
        {
            // Attempt mapping the colour buffer
            var cPtrBase = ParticleColourBuffer[updateIndex].MapBuffer(particleCount);

            if (cPtrBase == IntPtr.Zero)
                return;


            // Attempt mapping the matrix buffer
            var mPtrBase = ParticleMatrixBuffer[updateIndex].MapBuffer(particleCount);

            if (mPtrBase == IntPtr.Zero)
                return;


            var cPtr = (uint*)cPtrBase.ToPointer();
            var mPtr = (Matrix3F*)mPtrBase.ToPointer();


            for (int i = 0; i < THREAD_COUNT; i++)
            {
                // If this thread's particle array isn't empty
                if (t_ParticleCount[i] > 0)
                {
                    // Must allocate these variables here as Task.Run() may not execute till later
                    var index = i;
                    var threadExceptions = ThreadExceptions[index];
                    var bufferOffset = ParticleOffsets[index];

                    ParticleTasks.Add(Task.Run(() => UpdatePhysicalParticleRange(threadExceptions, cPtr, mPtr, index, bufferOffset, elapsed)));
                }
            }
        }


        unsafe void UpdatePhysicalParticleRange(List<Exception> threadExceptions, uint* cPtr, Matrix3F* mPtr, int index, int bufferOffset, float elapsed)
        {
            // This code is run on a separate thread, meaning any uncaught exceptions will crash the application.
            // Catch any exceptions here and add them to a list that we log to file on the UI thread later
            try
            {
                UpdatePhysicalParticleRangeInner(cPtr, mPtr, index, bufferOffset, elapsed);
            }
            catch (Exception e)
            {
                threadExceptions.Add(e);
            }
        }

        unsafe void UpdatePhysicalParticleRangeInner(uint* cPtr, Matrix3F* mPtr, int index, int bufferOffset, float elapsed)
        {
            // Precalculate
            var elapsedI = (int)elapsed;


            // Get a reference to the particle count and array for this thread
            ref int thisThreadCount = ref t_ParticleCount[index];
            var thisThreadArray = t_Particles[index];


            // Update every particle in this thread's array
            for (int i = thisThreadCount - 1; i >= 0; i--)
            {
                ref var q = ref thisThreadArray[i];


                q.lifetime -= elapsedI;



                // Remove particles when their lifetime expires
                if (q.lifetime <= 0)
                {
                    // Store an invisible particle in the particle buffer
                    cPtr[bufferOffset] = 0;
                    mPtr[bufferOffset++] = Matrix3F.Identity;


                    // Swap and pop
                    thisThreadArray[i] = thisThreadArray[--thisThreadCount];
                    continue;
                }


                // Example particle update - move and rotate
                var elapsedVelocity = q.velocity * elapsed;

                q.position += elapsedVelocity;
                q.rotation += elapsedVelocity * 0.25f;


                // Copy this particle's colour to shared memory
                cPtr[bufferOffset] = q.colour;

                
                // Precalculate for faster matrix calculation
                float sX = (float)Math.Sin(q.rotation.X);
                float cX = (float)Math.Cos(q.rotation.X);
                float sZ = (float)Math.Sin(q.rotation.Z);
                float cZ = (float)Math.Cos(q.rotation.Z);

                float a = q.scale;
                float acZ = a * cZ;
                float acX = a * cX;
                float asX = a * sX;


                // Get a reference to the matrix in shared GPU memory
                ref Matrix3F mat = ref mPtr[bufferOffset++];


                // Write to the matrix directly
                mat.M11 = acZ;
                mat.M12 = -a * sZ;

                mat.M13 = acX * sZ;
                mat.M14 = acX * cZ;
                mat.M21 = -asX;

                mat.M22 = asX * sZ;
                mat.M23 = acZ * sX;
                mat.M24 = acX;

                mat.M31 = q.position.X;
                mat.M32 = q.position.Y;
                mat.M33 = q.position.Z;
            }
        }
    }
}
