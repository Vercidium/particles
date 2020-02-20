using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Particles
{
    public static class ModelHelper
    {
        private const float epsilon = 0.001f;
        private const float minusHalfPI = (float)-Math.PI / 2;
        private const float halfPI = (float)Math.PI / 2;
        private const float doublePI = (float)Math.PI * 2;

        private static float[] sinWave;

        // Precalculate the sin wave
        public static void InitialiseTrigonometry()
        {
            int elements = (int)(Math.PI * 2 / epsilon) + 1;
            sinWave = new float[elements];

            int i = 0;
            for (double a = 0; a <= Math.PI * 2; a += epsilon)
            {
                sinWave[i] = (float)Math.Sin(a);
                i++;
            }
        }
        
        // Accessing an array is faster than using Math.sin() and Math.cos()
        // 1571 = PI / 2 / 0.0001
        // 6283 = PI * 2 / 0.0001
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetSinAndCosCheap(float t, out float sin, out float cos)
        {
            if (t < minusHalfPI)
            {
                int access = (int)(-t % doublePI / epsilon);
                sin = -sinWave[access];
                cos = -sinWave[(access + 1571) % 6283];
            }
            else if (t < 0)
            {
                sin = -sinWave[(int)(-t % doublePI / epsilon)];
                cos = sinWave[(int)((t + halfPI) % doublePI / epsilon)];
            }
            else
            {
                int access = (int)(t % doublePI / epsilon);
                sin = sinWave[access];
                cos = sinWave[(access + 1571) % 6283];
            }
        }
        
        // Rather than creating a scale, rotation and translation matrix and then combining them,
        // we can avoid lots of multiplications and divisions by creating the matrix in-place using algebra
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix4F ParticleMatrix(in float scale, in float rotationX, in float rotationZ, in Vector3 position)
        {
            // Get the sin and cos values for the X and Z rotation amount
            GetSinAndCosCheap(rotationX, out float sinX, out float cosX);
            GetSinAndCosCheap(rotationZ, out float sinZ, out float cosZ);

            Matrix4F m = Matrix4F.Identity;

            m.M11 = scale * cosZ;
            m.M12 = scale * sinZ;

            m.M21 = -scale * cosX * sinZ;
            m.M22 = scale * cosX * cosZ;
            m.M23 = scale * sinX;

            m.M31 = scale * sinX * sinZ;
            m.M32 = -scale * sinX * cosZ;
            m.M33 = scale * cosX;
            
            m.M41 = (float)position.X;
            m.M42 = (float)position.Y;
            m.M43 = (float)position.Z;

            return m;
        }

        // Create the particle cube
        public static ParticleVertexBuffer CreateParticleBuffer()
        {
            ParticleVertexBuffer buffer = new ParticleVertexBuffer();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                // Six vertices share the same normal
                var n = (byte)normals[i / 6];
                
                buffer.data[buffer.used++] = new ParticleVertex(vertices[triangles[i]],     n);
                buffer.data[buffer.used++] = new ParticleVertex(vertices[triangles[i + 1]], n);
                buffer.data[buffer.used++] = new ParticleVertex(vertices[triangles[i + 2]], n);
            }

            return buffer;
        }

        static int[] triangles =
        {
            2, 1, 0, // Front face
	        2, 0, 3,
            2, 3, 4, // Top face
	        2, 4, 5,
            1, 2, 5, // Right face
	        1, 5, 6,
            0, 7, 4, // Left face
	        0, 4, 3,
            5, 4, 7, // Back face
	        5, 7, 6,
            6, 7, 0, // Bottom face
	        6, 0, 1
        };

        static Vector3[] vertices =
        {
            new Vector3 (-1, -1, -1),
            new Vector3 (1,  -1, -1),
            new Vector3 (1,   1, -1),
            new Vector3 (-1,  1, -1),
            new Vector3 (-1,  1,  1),
            new Vector3 (1,   1,  1),
            new Vector3 (1,  -1,  1),
            new Vector3 (-1, -1,  1),
        };

        static FaceType[] normals =
        {
            FaceType.zn,
            FaceType.yp,
            FaceType.xp,
            FaceType.xn,
            FaceType.zp,
            FaceType.yn,
        };
    }
}
