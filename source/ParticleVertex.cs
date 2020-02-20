using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticleVertex
    {
        public ParticleVertex(in Vector3 position, byte normal)
        {
            PositionX = (byte)position.X;
            PositionY = (byte)position.Y;
            PositionZ = (byte)position.Z;
            Normal = normal;
        }

        public byte PositionX;
        public byte PositionY;
        public byte PositionZ;
        public byte Normal;
    }
}
