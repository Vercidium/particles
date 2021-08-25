using System;
using System.Runtime.InteropServices;

namespace Particles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3F
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3F Zero = new Vector3F(0, 0, 0);

        public bool IsEqual(Vector3F other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public static Vector3F operator +(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector3F operator -(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3F operator *(Vector3F a, Vector3F b)
        {
            return new Vector3F(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static Vector3F operator *(Vector3F a, float b)
        {
            return new Vector3F(a.X * b, a.Y * b, a.Z * b);
        }

        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public void Normalize()
        {
            float scale = 1.0f / Magnitude;
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

    }
}