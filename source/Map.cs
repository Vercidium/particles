using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Particles
{
    public class Map
    {
        public void RayMarch(in Vector3 start, Vector3 velocity, in double max, ref bool hit, ref Axis axis)
        {
            // Stub function - source code available at:
            // https://github.com/Vercidium/voxel-ray-marching/blob/master/source/Map.cs
        }

        public bool NoBlock(int x, int y, int z)
        {
            // Stub function - source code available at:
            // https://github.com/Vercidium/voxel-ray-marching/blob/master/source/Map.cs

            return false;
        }
    }
}
