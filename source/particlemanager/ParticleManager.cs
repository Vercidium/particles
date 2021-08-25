using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Particles
{
    public partial class ParticleManager
    {
        public ParticleManager()
        {
            GetThreadCount();


            // Set up 3 particle buffers for triple buffering
            for (int i = 0; i < TRIPLE_BUFFER_COUNT; i++)
            {
                var p = ParticleVertexBuffer[i] = ModelHelper.CreateParticleStrip();
                p.BufferData();

                ParticleColourBuffer[i] = new InstanceBuffer<uint>(p.vaoHandle, 1024, 1);
                ParticleMatrixBuffer[i] = new InstanceBuffer<Matrix3F>(p.vaoHandle, 1024, 2);
            }


            // Allocate storage for each thread. Start with 1024 particles in each array
            for (int i = 0; i < THREAD_COUNT; i++)
            {
                t_Particles[i] = new Particle[1024];
                ThreadExceptions[i] = new List<Exception>();
                t_ParticleCount[i] = 0;
            }
        }


    }
}
