using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Particles
{
    public partial class ParticleManager
    {
        const int MAX_PARTICLES_PER_THREAD = 32768;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckParticleSize<T>(ref T[] currentArray, int currentCount, int batchSize)
        {
            // If there isn't enough room
            if (currentCount + batchSize >= currentArray.Length)
            {
                // Increase the array by at least double, rather than lots of small increases
                var newLength = Math.Max(currentCount + batchSize, currentCount * 2);

                Array.Resize(ref currentArray, newLength);
            }
        }


        int TotalParticleCount()
        {
            int total = 0;

            for (int i = 0; i < THREAD_COUNT; i++)
                total += t_ParticleCount[i];

            return total;
        }


        // Each thread has its own particle storage
        Particle[][] t_Particles = new Particle[MAX_THREAD_COUNT][];


        // Each thread has its own particle count
        int[] t_ParticleCount = new int[MAX_THREAD_COUNT];


        // Track the amount of particles in each triple buffer slice
        int[] ParticleCounts = new int[TRIPLE_BUFFER_COUNT];


        // Track each thread's write offset into the same mapped VBO shared memory
        int[] ParticleOffsets = new int[MAX_THREAD_COUNT];


        // Store any exceptions that other threads have thrown
        List<Exception>[] ThreadExceptions = new List<Exception>[MAX_THREAD_COUNT];


        // Stores all 
        List<Task> ParticleTasks = new List<Task>();
    }
}
