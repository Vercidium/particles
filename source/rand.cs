using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Particles
{
    public static class rand
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> r =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Next(int max)
        {
            return r.Value.Next(max);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextFloat()
        {
            return (float)r.Value.NextDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3F NextV3F(float min, float max)
        {
            float diff = max - min;
            return new Vector3F(NextFloat() * diff + min, NextFloat() * diff + min, NextFloat() * diff + min);
        }
    }
}
