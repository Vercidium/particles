using System;

namespace Particles
{
    public struct Particle
    {
        // Base variables
        public Vector3F position;
        public Vector3F rotation;
        public Vector3F velocity;
        public uint colour;
        public int lifetime;
        public float scaleModifier;

        
        // Shortcuts
        public float scale => Math.Min(1, lifetime / 1000.0f) * scaleModifier;
        

        // In-place reset constructor
        // Rotation isn't set as I don't mind which way a particle is facing when it's created
        public void Reset(Vector3F p, Vector3F v, int l, float sm, uint c)
        {
            position = p;
            velocity = v;
            scaleModifier = sm;
            colour = c;
            lifetime = l;
        }
    }
}
