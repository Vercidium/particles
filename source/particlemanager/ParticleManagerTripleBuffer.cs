namespace Particles
{
    public partial class ParticleManager
    {
        public const int TRIPLE_BUFFER_COUNT = 3;

        public int CurrentTripleBufferIndex;
        public bool CanRenderParticles = false;


        public void IncrementTripleBufferIndex()
        {
            CurrentTripleBufferIndex++;


            // Handle wrap around
            if (CurrentTripleBufferIndex >= TRIPLE_BUFFER_COUNT)
            {
                CurrentTripleBufferIndex = 0;


                // Once all 3 buffers are populated, we can start rendering particles
                CanRenderParticles = true;
            }
        }

        int TripleBufferOffset(int offset)
        {
            int r = CurrentTripleBufferIndex + offset;


            // Handle wrap around
            if (r >= TRIPLE_BUFFER_COUNT)
                r -= TRIPLE_BUFFER_COUNT;


            return r;
        }


        // Shortcuts
        public int ParticleRenderIndex => TripleBufferOffset(0);
        public int ParticleUpdateIndex => TripleBufferOffset(2);
    }
}
