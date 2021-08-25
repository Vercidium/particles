namespace Particles
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialise the particle engine
            var PM = new ParticleManager();


            // Add some particles
            PM.ExampleBatchAdd();


            // Mock game loop
            while (true)
            {
                PM.PreRenderUpdate();
                PM.RenderParticles();
                PM.UpdateParticles(16.0f);
            }
        }
    }
}
