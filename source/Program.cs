namespace Particles
{
    class Program
    {
        static void Main(string[] args)
        {
            var PM = new ParticleManager();

            PM.ExampleBatchAdd();


            // Game loop
            while (true)
            {
                PM.PreRenderUpdate();
                PM.RenderParticles();
                PM.UpdateParticles(16.0f);
            }
        }
    }
}
