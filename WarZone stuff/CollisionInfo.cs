using System.Collections.Generic;

namespace WarZoneLib
{
    public class CollisionInfo
    {
        public Dictionary<uint, FixtureInfo> Fixtures = new Dictionary<uint, FixtureInfo>();
        public Vector3[] Vertices;
        public TriangleInfo[] Triangles;
        public BSPNodeInfo BSP;
    }
}
