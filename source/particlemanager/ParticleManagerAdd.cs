using System.Runtime.CompilerServices;

namespace Particles
{
    public partial class ParticleManager
    {
        public void ExampleBatchAdd()
        {
            var batchSize = 100;


            // Determine which thread's storage to add this batch to
            var index = CurrentThreadAddIndex;


            // Get references to this thread's storage and particle count
            ref var currentArray = ref t_Particles[index];
            ref var currentAmount = ref t_ParticleCount[index];


            // Ensure there is enough room in this thread's particle storage for this batch
            CheckParticleSize(ref currentArray, currentAmount, batchSize);


            // Purple RGBA
            var colour = (uint)((255 << 32) | (0 << 24) | (255 << 16) | 255); 


            // Particle scale
            var scaleModifier = 0.5f;


            // Create 100 purple particles at random positions with random velocity
            for (int i = 0; i < batchSize; i++)
            {
                var position = Rand.NextV3F(-3.0f, 3.0f);
                var velocity = Rand.NextV3F(-0.01f, 0.01f);
                var lifetime = Rand.Next(5000);

                AddParticle(ref currentArray,
                            ref currentAmount,
                            position,
                            velocity,
                            lifetime,
                            scaleModifier,
                            colour);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddParticle(ref Particle[] currentArray, ref int currentCount, in Vector3F position, in Vector3F velocity, in int lifetime, in float scaleModifier, in uint colour)
        {
            // Add to the end of the array. If the array is full, replace a random particle
            var index = currentCount > MAX_PARTICLES_PER_THREAD ? Rand.Next(currentCount) : currentCount++;

            currentArray[index].Reset(
                    position,
                    velocity,
                    lifetime,
                    scaleModifier,
                    colour);
        }
    }
}
