using System.Runtime.InteropServices;

namespace Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticleVertex
    {
        public ParticleVertex(in Vector3F position, byte normal)
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
