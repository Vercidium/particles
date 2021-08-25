using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Particles
{
    public partial class ParticleManager
    {
        // OpenGL Data
        static ParticleVertexBuffer[] ParticleVertexBuffer = new ParticleVertexBuffer[TRIPLE_BUFFER_COUNT];
        static InstanceBuffer<uint>[] ParticleColourBuffer = new InstanceBuffer<uint>[TRIPLE_BUFFER_COUNT];
        static InstanceBuffer<Matrix3F>[] ParticleMatrixBuffer = new InstanceBuffer<Matrix3F>[TRIPLE_BUFFER_COUNT];


        public void PreRenderUpdate()
        {
            if (ParticleTasks.Count > 0)
            {
                // Wait for any threads that may be running
                Task.WaitAll(ParticleTasks.ToArray());


                // Unmap previously mapped buffers
                var updateIndex = ParticleUpdateIndex;
                ParticleColourBuffer[updateIndex].UnmapBuffer();
                ParticleMatrixBuffer[updateIndex].UnmapBuffer();


                // Advance to the next triple buffer slice
                IncrementTripleBufferIndex();
            }
        }


        public void RenderParticles()
        {
            // We must wait before all 3 particle buffers are initialised before rendering
            if (!CanRenderParticles)
                return;


            // ------------------------------
            // Bind your particle shader here
            // ------------------------------

            var renderIndex = ParticleRenderIndex;
            var renderCount = ParticleCounts[renderIndex];


            Console.WriteLine($"Rendering {renderCount} particles");

            // Bind the VAO for the buffer we are rendering this frame
            Gl.BindVertexArray(ParticleMatrixBuffer[renderIndex].vaoHandle);

            // Draw the particles
            Gl.DrawArraysInstanced(PrimitiveType.Triangles, 0, 14, renderCount);

            // Unbind the VAO
            Gl.BindVertexArray(0);
        }
    }
}
