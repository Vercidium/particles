using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particles
{
    public enum FaceType : byte
    {
        yp = 0,
        yn,
        xp,
        xn,
        zp,
        zn,
    }

    public enum Axis
    {
        None = 0,
        X,
        Y,
        Z,
    }

    public static class Constants
    {
        public const double Gravity = -0.000025;

        public const double ParticleSlideDampening = 0.5;
        public const double ParticleDampening = 0.9;
        public const double ParticleBounce = -0.35;

        public static Vector3[] AxisToBounce = new Vector3[4]
        {
            new Vector3(ParticleDampening, ParticleDampening, ParticleDampening),
            new Vector3(ParticleBounce,    ParticleDampening, ParticleDampening),
            new Vector3(ParticleDampening, ParticleBounce,    ParticleDampening),
            new Vector3(ParticleDampening, ParticleDampening, ParticleBounce)
        };
    }
}
