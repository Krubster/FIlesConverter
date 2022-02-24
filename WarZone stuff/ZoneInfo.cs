namespace WarZoneLib
{
    public class ZoneInfo
    {
        public uint ID;
        public uint OffsetX;
        public uint OffsetY;
        public CollisionInfo Collision;
        public string Name;
        public bool Enabled;
        public TerrainInfo Terrain;

        public ZoneInfo()
        {
            Enabled = true;
        }
    }
}
