using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particles
{
    public class Particle
    {
        // Base particle values
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 rotation;
        public float scale;
        public uint colour;

        // As an optimisation, the transform matrix can be precalculated and stored here
        public Matrix4F transform;

        // When this value reaches 0, the particle has decayed
        public float lifeTime;

        // The linked list
        public Particle Next;

        public void AddNext(Particle p)
        {
            if (Next == null)
            {
                // Start the linked list
                Next = p;
            }
            else
            {
                // Insert the particle at the start of the linked list
                p.Next = Next;
                Next = p;
            }
        }

        // Remove the particle at the start of the linked list
        public void PopNext()
        {
            Next = Next.Next;
        }
    }
}
