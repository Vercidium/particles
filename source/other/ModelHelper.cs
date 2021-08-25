namespace Particles
{
    public static class ModelHelper
    {
        public const int CUBE_STRIP_LENGTH = 14;

        public static ParticleVertexBuffer CreateParticleStrip()
        {
            var buffer = new ParticleVertexBuffer();

            for (int i = 0; i < CUBE_STRIP_LENGTH; i++)
            {
                var n = (byte)directionStrip[i];

                var pos = centeredVertices[triangleStrip[i]];
                buffer.Append(new ParticleVertex(pos, n));
            }

            return buffer;
        }

        // Particle cubes are composed of 14 triangles in a strip
        static int[] triangleStrip =
        {
            3, 2, 0, 1,
            6, 2, 5, 3,
            4, 0, 7, 6,
            4, 5,
        };

        static FaceType[] directionStrip =
        {
            FaceType.zn, FaceType.zn, FaceType.zn, FaceType.zn,
            FaceType.yn, FaceType.xp, FaceType.xp, FaceType.yp,
            FaceType.yp, FaceType.xn, FaceType.xn, FaceType.yn,
            FaceType.zp, FaceType.zp,
        };

        static Vector3F[] centeredVertices =
        {
            new Vector3F(-1,-1, -1),
            new Vector3F(1, -1, -1),
            new Vector3F(1, 1, -1),
            new Vector3F(-1, 1, -1),
            new Vector3F(-1, 1, 1),
            new Vector3F(1, 1, 1),
            new Vector3F(1, -1, 1),
            new Vector3F(-1, -1, 1),
        };
    }
}
