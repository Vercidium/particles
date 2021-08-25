using System.Runtime.InteropServices;

namespace Particles
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3F
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;

        public Matrix3F(float m11, float m12, float m13, float m14,
                          float m21, float m22, float m23, float m24,
                          float m31, float m32, float m33, float m34)
        {
            M11 = m11; M12 = m12; M13 = m13; M14 = m14;
            M21 = m21; M22 = m22; M23 = m23; M24 = m24;
            M31 = m31; M32 = m32; M33 = m33; M34 = m34;
        }

        public static Matrix3F Identity = new Matrix3F(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    }
}
