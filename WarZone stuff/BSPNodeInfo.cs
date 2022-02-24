namespace WarZoneLib
{
    public class BSPNodeInfo
    {
        public Plane P;
        public BSPNodeInfo Back;
        public BSPNodeInfo Front;
        public int[] Triangles;

        public bool IsLeaf
        {
            get
            {
                return Back == null;
            }
        }
    }
}
