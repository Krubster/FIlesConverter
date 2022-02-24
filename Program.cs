using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Niflib;
using Niflib.Extensions;
using OpenTK;
using FreeImageAPI;
using System.Xml.Linq;
using System.Xml;
using System.Drawing.Imaging;
using Structures.TriangleNet.Geometry;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Structures.TriangleNet;
using Structures.TriangleNet.Meshing;
using WarZoneLib;

namespace FIlesConverter
{

    public enum VertexType
    {
        Valid,
        Hole
    }
    public struct TerrainVertex
    {
        public int ID;
        public float X;
        public float Y;
        public float Z;
        public VertexType Type;

        public TerrainVertex(int id, float x, float y, float z, VertexType type)
        {
            ID = id;
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }
    }

    public struct TerrainPolygon
    {
        public TerrainVertex V1, V2, V3;

        public TerrainPolygon(TerrainVertex v1, TerrainVertex v2, TerrainVertex v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }
    }

    public class TerrainMesh
    {
        public TerrainVertex[,] HeightMap;
        public List<TerrainPolygon> Polygons;

        public TerrainMesh(int width, int height)
        {
            Polygons = new List<TerrainPolygon>(width * height * 2);
            HeightMap = new TerrainVertex[width, height];
            int id = 0;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    HeightMap[x, y] = new TerrainVertex(id++, 0f, 0f, 0f, VertexType.Hole);
                }
            }
        }

        public float[] BuildVertices()
        {
            List<float> verts = new List<float>();
            for (int x = 0; x < HeightMap.GetLength(0); ++x)
            {
                for (int y = 0; y < HeightMap.GetLength(1); ++y)
                {
                    TerrainVertex vert = HeightMap[x, y];
                    if (vert.Type != VertexType.Hole)
                    {
                        verts.Add(vert.X); verts.Add(vert.Y); verts.Add(vert.Z);
                    }
                }
            }
            return verts.ToArray();
        }

        public int[] BuildIndicies()
        {
            int[] arr = new int[Polygons.Count * 3];
            int v = 0;
            foreach (TerrainPolygon poly in Polygons)
            {
                arr[v++] = poly.V1.ID + 1;
                arr[v++] = poly.V2.ID + 1;
                arr[v++] = poly.V3.ID + 1;
            }
            return arr;
        }
    }

    public class WaterBody
    {
        public float[] Vertices { get; private set; }
        public int[] Polygons { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        public WaterBody()
        {
        }

        public WaterBody(Mesh mesh, float x, float y, float z, string name, string type)
        {
            Name = name;
            Type = type;
            X = x;
            Y = y;
            Z = z;
            Vertices = new float[mesh.Vertices.Count * 3];
            int i = 0;
            foreach (Vertex vert in mesh.Vertices)
            {
                Vertices[i++] = (float)vert.X;
                Vertices[i++] = (float)vert.Y;
                Vertices[i++] = 0.0f;
            }
            i = 0;
            Polygons = new int[mesh.Triangles.Count * 3];
            foreach (Structures.TriangleNet.Topology.Triangle tri in mesh.Triangles)
            {
                Polygons[i++] = tri.GetVertexID(0); // ???
                Polygons[i++] = tri.GetVertexID(1);
                Polygons[i++] = tri.GetVertexID(2);
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(Type);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);

            writer.Write(Vertices.Length);
            for (int i = 0; i < Vertices.Length; ++i)
            {
                writer.Write(Vertices[i]);
            }
            writer.Write(Polygons.Length);
            for (int i = 0; i < Polygons.Length; ++i)
            {
                writer.Write(Polygons[i]);
            }
        }

        public void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            Type = reader.ReadString();
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Z = reader.ReadSingle();
            int amt = reader.ReadInt32();
            Vertices = new float[amt];
            for (int i = 0; i < amt; ++i)
            {
                Vertices[i] = reader.ReadSingle();
            }
            amt = reader.ReadInt32();
            Polygons = new int[amt];
            for (int i = 0; i < amt; ++i)
            {
                Polygons[i] = reader.ReadInt32();
            }
        }
    }

    public class TerrainPiece : ObjModel
    {
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        public TerrainPiece(float x, float y)
        {
            XOffset = x;
            YOffset = y;
        }

        internal void RemoveOffset()
        {
            for (int i = 0; i < Vertices.Length; i += 3)
            {
                Vertices[i] -= XOffset;
                Vertices[i + 1] -= YOffset;
            }
        }
    }

    public class ObjModel
    {
        public float[] Vertices { get; set; }
        public int[] Polygons { get; set; }

        public ObjModel()
        {

        }

        public ObjModel(float[] verts, int[] tris)
        {
            Vertices = verts;
            Polygons = tris;
        }

        public ObjModel(List<float> verts, List<int> tris)
        {
            Vertices = verts.ToArray();
            Polygons = tris.ToArray();
        }

        public ObjModel(Mesh mesh)
        {
        }

        public virtual void Serialize(string path)
        {
            using (BinaryWriter writer = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                writer.Write(Vertices.Length);
                for (int i = 0; i < Vertices.Length; ++i)
                {
                    writer.Write(Vertices[i]);
                }
                writer.Write(Polygons.Length);
                for (int i = 0; i < Polygons.Length; ++i)
                {
                    writer.Write(Polygons[i]);
                }
            }
        }

        public virtual void SerializeObj(string path)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                for (int i = 0; i < Vertices.Length; i += 3)
                {
                    writer.WriteLine($"v {Vertices[i].ToString().Replace(',', '.')} {Vertices[i + 1].ToString().Replace(',', '.')} {Vertices[i + 2].ToString().Replace(',', '.')}");
                }
                for (int i = 0; i < Polygons.Length; i += 3)
                {
                    writer.WriteLine($"f {Polygons[i]} {Polygons[i + 1]} {Polygons[i + 2]}");
                }
            }
        }

        public virtual void Deserialize(string path)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                int amt = reader.ReadInt32();
                Vertices = new float[amt];
                for (int i = 0; i < amt; ++i)
                {
                    Vertices[i] = reader.ReadSingle();
                }
                amt = reader.ReadInt32();
                Polygons = new int[amt];

                for (int i = 0; i < amt; ++i)
                {
                    Polygons[i] = reader.ReadInt32();
                }
            }
        }

        public void Deserialize(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            using (BinaryReader reader = new BinaryReader(ms))
            {
                int amt = reader.ReadInt32();
                Vertices = new float[amt];
                byte[] arr = reader.ReadBytes(amt * sizeof(float));
                Buffer.BlockCopy(arr, 0, Vertices, 0, arr.Length);

                amt = reader.ReadInt32();
                Polygons = new int[amt];
                arr = reader.ReadBytes(amt * sizeof(int));
                Buffer.BlockCopy(arr, 0, Polygons, 0, arr.Length);
            }
            ms.Close();
            ms.Dispose();
        }

        public virtual void DeserializeObj(string path)
        {
            string[] lines = File.ReadAllLines(path);
            var vertices = new List<float>();
            var polygons = new List<int>();
            foreach (string line in lines)
            {
                // строки с вершинами
                if (line.ToLower().StartsWith("v "))
                {
                    var vx = line.Split(' ')
                                 .Skip(1)
                                 .Select(v => Single.Parse(v.Replace('.', ',')))
                                 .ToArray();
                    vertices.Add(vx[0]);
                    vertices.Add(vx[1]);
                    vertices.Add(vx[2]);
                }
                // строки с номерами
                else if (line.ToLower().StartsWith("f"))
                {
                    var vx = line.Split(' ')
                                 .Skip(1)
                                 .Select(v => Int32.Parse(v.Split('/')[0]))
                                 .ToArray();
                    polygons.Add(vx[0]);
                    polygons.Add(vx[1]);
                    polygons.Add(vx[2]);
                }
            }
            Vertices = vertices.ToArray();
            Polygons = polygons.ToArray();
        }

        public void Refine()
        {
            for (int i = 0; i < Polygons.Length; ++i)
            {
                Polygons[i] -= 1;
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NifInfo
    {
        public int ID;
        public float MinAngle;
        public float MaxAngle;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string ModelName;

        public NifInfo(int id, string model, float minAngle, float maxAngle)
        {
            ID = id;
            ModelName = model;
            MinAngle = minAngle;
            MaxAngle = maxAngle;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct FixtureInfo
    {
        public byte InstanceID;
        public ushort UniqueID;
        public int ID;
        public int NifID;
        public int O;
        public int Scale;
        public double Angle3D;
        public double XAxis;
        public double YAxis;
        public double ZAxis;
        public double X;
        public double Y;
        public double Z;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Name;


        public FixtureInfo(int id, ushort uniID, byte instance, int nifID, string name, double x, double y, double z, int o, int scale, double angle3D, double xAxis, double yAxis, double zAxis)
        {
            UniqueID = uniID;
            InstanceID = instance;
            ID = id;
            NifID = nifID;
            Name = name;
            X = x;
            Y = y;
            Z = z;
            O = o;
            Scale = scale;
            Angle3D = angle3D;
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            bool pack = false;
            bool serialize = false;
            bool serializeTerrain = false;
            bool zones = false;
            bool water = false;
            bool reverse = false;
            bool simplify = false;
            bool nifToObj = false;
            bool packWarZone = false;
            string pathIn = "";
            string pathOut = "";
            var p = new OptionSet() {
                   { "in=",               "", v => pathIn = v },
                   { "out=",              "", v => pathOut = v  },
                   { "serialize",         "", v => serialize = v != null },
                   { "serializeTerrain",  "", v => serializeTerrain = v != null },
                   { "pack",              "", v => pack = v != null },
                   { "zones",             "", v => zones = v != null },
                   { "water",             "", v => water = v != null },
                   { "nifToObj",          "", v => nifToObj = v != null },
                   { "reverse",           "", v => reverse = v != null },
                   { "simplify",          "", v => simplify = v != null },
                   { "packWZ",            "", v => packWarZone = v != null },
            };
            List<string> extra = p.Parse(args);
            Console.WriteLine($"- Pack WarZone: {packWarZone}");
            Console.WriteLine($"- Pack: {pack}");
            Console.WriteLine($"- Serialize: {serialize}");
            Console.WriteLine($"- Zones: {zones}");
            Console.WriteLine($"- Simplify: {simplify}");
            Console.WriteLine($"- Serialize Terrain: {serializeTerrain}");
            Console.WriteLine($"- Reverse: {reverse}");
            Console.WriteLine($"- Nif-To-Obj: {nifToObj}");
            Console.WriteLine($"- Water: {water}");
            Console.WriteLine($"- Input: {pathIn}");
            Console.WriteLine($"- Output: {pathOut}");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            if (!Directory.Exists(pathIn))
            {
                Console.WriteLine("Wrong source path!");
                return;
            }
            if (!Directory.Exists(pathOut))
            {
                Console.WriteLine("Created output bin directory at: " + pathOut);
                Directory.CreateDirectory(pathOut);
            }
            if (packWarZone)
            {
                PerformPackWZ(pathIn, pathOut);
            }
            if (serializeTerrain)
            {
                PerformSerializeTerrain(pathIn, pathOut);
            }
            if (water)
            {
                PerformWater(pathIn, pathOut);
            }
            if (nifToObj)
            {
                PerformNifToObj(pathIn, pathOut);
            }
            if (reverse)
            {
                PerformReverse(pathIn, pathOut);
            }
            if (pack)
            {
                PerformPack(pathIn, pathOut);
            }
            if (serialize)
            {
                PerformSerialize(pathIn, pathOut);
            }
            if (zones)
            {
                PerformZones(pathIn, pathOut);
            }
            if (simplify)
            {
                PerformSimplify(pathIn, pathOut);
            }
            Console.WriteLine();
            Console.WriteLine("Done!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        //Dumb a little, searches output dir of all previous actions (requires zoneInfo.bin, water.bin, terrain.pcx, offset.pcx, holemap.pcx, sector.dat in every zone folder and fixtures.bin, zones.dat, zoneinfo.txt in root folder)
        private static void PerformPackWZ(string pathIn, string pathOut)
        {
            string[] dirs = Directory.GetDirectories(pathIn);
            int processed = 0;
            Dictionary<string, ObjModel> models = new Dictionary<string, ObjModel>();
            if (!File.Exists(Path.Combine(@"fixtures.bin")))
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[OCCLUSION]: No fixtures.bin in root folder!");
                Console.ForegroundColor = oldColor;
                return;
            }
            using (BinaryReader reader = new BinaryReader(new FileStream(Path.Combine(@"fixtures.bin"), FileMode.Open)))
            {
                byte version = reader.ReadByte();
                int amt = reader.ReadInt32();
                for (int i = 0; i < amt; ++i)
                {
                    string name = reader.ReadString();
                    byte[] bytes = reader.ReadBytes(reader.ReadInt32());
                    ObjModel model = new ObjModel();
                    model.Deserialize(bytes);
                    if (model.Vertices.Length > 0 && model.Polygons.Length > 0)
                    {
                        models.Add(name, model);
                    }
                    else
                    {
                        ConsoleColor oldColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[OCCLUSION]: Model {name} has no faces or polygons!");
                        Console.ForegroundColor = oldColor;

                    }
                }
            }
            Dictionary<int, int> Regions = new Dictionary<int, int>();

            //Loading data
            using (StreamReader reader = new StreamReader(new FileStream(Path.Combine("zones.dat"), FileMode.Open)))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("[zone"))
                    {
                        line = line.Remove(8);
                        line = line.Remove(0, 5);
                        line = line.TrimStart('0');
                        int zId = Convert.ToInt32(line);
                        while (!(line = reader.ReadLine()).Contains("region="))
                        {
                        }
                        line = line.Split('=')[1];
                        int region = Convert.ToInt32(line);
                        if (!Regions.ContainsKey(zId))
                        {
                            Regions.Add(zId, region);
                        }
                    }
                }
            }

            Dictionary<int, int[]> Offsets = new Dictionary<int, int[]>();
            using (StreamReader reader = new StreamReader(new FileStream(Path.Combine("zoneinfo.csv"), FileMode.Open)))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    int zoneId = Convert.ToInt32(line.Split(',')[0]);
                    int xOff = Convert.ToInt32(line.Split(',')[1]);
                    int yOff = Convert.ToInt32(line.Split(',')[2]);
                    Offsets.Add(zoneId, new int[] { xOff, yOff });
                }
            }
            foreach (var dir in dirs)
            {
                ++processed;
                Console.WriteLine($"Packing: {dir}");
                string num = Path.GetFileName(dir).Replace("zone", "");
                num = RemoveZeros(num);
                int zoneID = int.Parse(num);
                if (!File.Exists(Path.Combine(pathOut, $"{zoneID}.bin")))
                {
                    PackZoneOld(models, Regions[zoneID], Offsets[zoneID][0], Offsets[zoneID][1], dir, pathOut);
                }
                Console.WriteLine($"Packed zone #{zoneID}");
            }
        }

        private static T BytesToStruct<T>(byte[] rawData) where T : struct
        {
            T result = default(T);
            try
            {
                IntPtr rawDataPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(rawData.Length);
                System.Runtime.InteropServices.Marshal.Copy(rawData, 0, rawDataPtr, rawData.Length);

                result = (T)System.Runtime.InteropServices.Marshal.PtrToStructure(rawDataPtr, typeof(T));
                System.Runtime.InteropServices.Marshal.FreeHGlobal(rawDataPtr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        private static void PackZoneOld(Dictionary<string, ObjModel> models, int regionId, int xOff, int yOff, string dir, string pathOut)
        {
            string num = Path.GetFileName(dir).Replace("zone", "");
            num = RemoveZeros(num);
            int zoneID = int.Parse(num);

            //Data to load
            Dictionary<int, FixtureInfo> FixtureInfos = new Dictionary<int, FixtureInfo>();
            Dictionary<int, NifInfo> ModelInfos = new Dictionary<int, NifInfo>();
            List<WaterBody> bodies = new List<WaterBody>();
            ushort[,] HeightMap;
            bool[,] HoleMap;

            //Loading data
            int scaleFactor = 0;
            int offsetFactor = 0;
            byte[,] _OffsetData;
            byte[,] _TerrainData;

            using (StreamReader reader = new StreamReader(new FileStream(Path.Combine(dir, "sector.dat"), FileMode.Open)))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("scalefactor"))
                    {
                        scaleFactor = Convert.ToInt32(line.Split('=')[1]);
                    }
                    if (line.Contains("offsetfactor"))
                    {
                        offsetFactor = Convert.ToInt32(line.Split('=')[1]);
                    }
                }
            }

            FreeImageAPI.FreeImageBitmap fiBitmap = new FreeImageAPI.FreeImageBitmap(new FileStream(Path.Combine(dir, "offset.pcx"), FileMode.Open));
            Bitmap bitmap = fiBitmap.ToBitmap();
            _OffsetData = new byte[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    _OffsetData[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
            bitmap.Dispose();

            fiBitmap = new FreeImageAPI.FreeImageBitmap(new FileStream(Path.Combine(dir, "terrain.pcx"), FileMode.Open));
            bitmap = fiBitmap.ToBitmap();
            _TerrainData = new byte[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    _TerrainData[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
            bitmap.Dispose();
            if (!File.Exists(Path.Combine(dir, "holemap.pcx")))
            {
                if (File.Exists(Path.Combine(dir, "holemap.png")))
                {
                    File.Move(Path.Combine(dir, "holemap.png"), Path.Combine(dir, "holemap.pcx"));
                }
            }
            HoleMap = new bool[_TerrainData.GetLength(0), _TerrainData.GetLength(1)];

            for (int x = 0; x < HoleMap.GetLength(0); ++x)
            {
                for (int y = 0; y < HoleMap.GetLength(1); ++y)
                {
                    HoleMap[x, y] = false;
                }
            }

            if (File.Exists(Path.Combine(dir, "holemap.pcx")))
            {
                fiBitmap = new FreeImageAPI.FreeImageBitmap(new FileStream(Path.Combine(dir, "holemap.pcx"), FileMode.Open), FREE_IMAGE_FORMAT.FIF_PCX, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
                fiBitmap.Rescale(new Size(_TerrainData.GetLength(0), _TerrainData.GetLength(1)), FREE_IMAGE_FILTER.FILTER_BOX);
                bitmap = fiBitmap.ToBitmap();

                for (int x = 0; x < bitmap.Width; ++x)
                {
                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        byte pixel = bitmap.GetPixel(x, y).R;
                        if (pixel == 0)
                        {
                            HoleMap[x, y] = true;
                        }
                    }
                }
                bitmap.Dispose();
                fiBitmap.Dispose();
            }

            HeightMap = new ushort[_TerrainData.GetLength(0), _TerrainData.GetLength(1)];
            for (int x = 0; x < _TerrainData.GetLength(0); x++)
            {
                for (int y = 0; y < _TerrainData.GetLength(1); y++)
                {
                    HeightMap[x, y] = (ushort)(scaleFactor * _TerrainData[x, y] + offsetFactor * _OffsetData[x, y]);
                }
            }

            if (File.Exists(Path.Combine(dir, "zoneInfo.bin"))) //new fast reading
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(Path.Combine(dir, "zoneInfo.bin"), FileMode.Open)))
                {
                    byte version = reader.ReadByte();

                    int count = reader.ReadInt32();
                    int structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(NifInfo));
                    byte[] arr = new byte[structSize];
                    for (int i = 0; i < count; ++i)
                    {
                        reader.Read(arr, 0, arr.Length);
                        NifInfo info = BytesToStruct<NifInfo>(arr);
                        ModelInfos.Add(info.ID, info);
                    }
                    count = reader.ReadInt32();
                    structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(FixtureInfo));
                    arr = new byte[structSize];
                    for (int i = 0; i < count; ++i)
                    {
                        reader.Read(arr, 0, arr.Length);
                        FixtureInfo info = BytesToStruct<FixtureInfo>(arr);
                        FixtureInfos.Add(info.ID, info);
                    }
                }
            }
            GC.Collect();

            if (File.Exists(Path.Combine(dir, "water.bin")))
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(Path.Combine(dir, "water.bin"), FileMode.Open)))
                {
                    byte version = reader.ReadByte();
                    int count = reader.ReadInt32();
                    bodies.Capacity = count;
                    for (int i = 0; i < count; ++i)
                    {
                        WaterBody body = new WaterBody();
                        body.Deserialize(reader);
                        bodies.Add(body);
                    }
                }
            }
            GC.Collect();

            //Writing data
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathOut, $"{zoneID}.bin"), FileMode.Create)))
            {
                //Header
                WriteHeader(writer);

                //Write region info
                MemoryStream ms = new MemoryStream();
                using (BinaryWriter writer2 = new BinaryWriter(ms))
                {
                    writer2.Write(regionId); //region id
                    writer2.Write(1);

                    writer2.Write(zoneID);
                    writer2.Write(xOff << 12);
                    writer2.Write(yOff << 12);
                    writer2.Write(0); //nif count
                    writer2.Write(0); // fixtures count
                    WriteChunk(writer, ChunkType.Region, ms);
                }


                //Write Terrain info
                ms = new MemoryStream();
                using (BinaryWriter writer2 = new BinaryWriter(ms))
                {
                    writer2.Write(regionId); //region id
                    writer2.Write(zoneID);

                    writer2.Write(HeightMap.GetLength(0));
                    writer2.Write(HeightMap.GetLength(1));
                    writer2.Write(HoleMap.GetLength(0));
                    writer2.Write(HoleMap.GetLength(1));

                    for (int i = 0; i < HeightMap.GetLength(0); ++i)
                    {
                        for (int j = 0; j < HeightMap.GetLength(1); ++j)
                        {
                            writer2.Write((ushort)HeightMap[i, j]);
                        }
                    }
                    for (int i = 0; i < HoleMap.GetLength(0); ++i)
                    {
                        for (int j = 0; j < HoleMap.GetLength(1); ++j)
                        {
                            if (HoleMap[i, j])
                            {
                                writer2.Write((byte)0);
                            }
                            else
                            {
                                writer2.Write((byte)1);
                            }
                        }
                    }
                    WriteChunk(writer, ChunkType.Terrain, ms);
                }


                //Write Collision info
                ms = new MemoryStream();
                using (BinaryWriter writer2 = new BinaryWriter(ms))
                {
                    writer2.Write((uint)regionId);
                    writer2.Write((uint)zoneID);

                    List<float> vertices = new List<float>();
                    List<int> indexes = new List<int>();
                    List<int> indexesExport = new List<int>();

                    foreach (int key in FixtureInfos.Keys)
                    {
                        FixtureInfo info = FixtureInfos[key];
                        if (!ModelInfos.ContainsKey(info.NifID))
                        {
                            Console.WriteLine($"Missing fixture info: {info.NifID} - {info.Name} in {dir}");
                            continue;
                        }
                        if (!models.ContainsKey(ModelInfos[info.NifID].ModelName))
                        {
                            //  Log.Debug("[OCCLUSION]", $"Missing model: {ModelInfos[info.NifID].ModelName}");
                            continue;
                        }
                        NifInfo nif = ModelInfos[info.NifID];
                        ObjModel model = models[nif.ModelName];
                        Matrix startTransform = Matrix.Translation( //translation
                                65535.0f - (float)info.X,
                                (float)info.Y,
                                (float)info.Z
                            );

                        Matrix rotMatrix = Matrix.RotationAxis(new WarZoneLib.Vector3(-(float)info.XAxis, (float)info.YAxis, (float)info.ZAxis), (float)info.Angle3D);
                        float angle2D = Clamp(info.O, nif.MinAngle, nif.MinAngle) / 180.0f * (float)Math.PI;
                        Matrix rot2D = Matrix.RotationZ(angle2D);
                        rotMatrix = rot2D * rotMatrix * Matrix.RotationZ((float)Math.PI);
                        Matrix scaleMatrix = Matrix.Scaling(info.Scale / 100.0f, info.Scale / 100.0f, info.Scale / 100.0f);
                        startTransform = scaleMatrix * rotMatrix * startTransform;

                        int baseIndex = vertices.Count / 3;

                        for (int i = 0; i < model.Vertices.Length; i += 3)
                        {
                            WarZoneLib.Vector3 result = startTransform.TransformPoint(new WarZoneLib.Vector3(model.Vertices[i], model.Vertices[i + 1], model.Vertices[i + 2]));
                            vertices.Add(result.X);
                            vertices.Add(result.Y);
                            vertices.Add(result.Z);
                        }

                        ushort uniqueID = info.UniqueID;
                        ushort _zoneId = (ushort)zoneID;
                        byte instance = info.InstanceID;

                        int data = (int)
                            (
                                (int)((uniqueID & 0xC000) << 16) |
                                (int)((_zoneId & 0x3FF) << 20) |
                                (int)((uniqueID & 0x3FFF) << 6) |
                                (int)(0x28 + instance)
                                );

                        for (int i = 0; i < model.Polygons.Length; i += 3)
                        {
                            indexes.Add(model.Polygons[i + 0] + baseIndex - 1);
                            indexes.Add(model.Polygons[i + 1] + baseIndex - 1);
                            indexes.Add(model.Polygons[i + 2] + baseIndex - 1);
                            indexes.Add(data);

                            indexesExport.Add(model.Polygons[i + 0] + baseIndex);
                            indexesExport.Add(model.Polygons[i + 1] + baseIndex);
                            indexesExport.Add(model.Polygons[i + 2] + baseIndex);
                        }
                    }

                    ObjModel model2 = new ObjModel(vertices, indexesExport);
                    model2.SerializeObj(Path.Combine(pathOut, $"{zoneID}-fixtures.obj"));

                    writer2.Write((int)(vertices.Count / 3));

                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        writer2.Write((float)vertices[i]);
                    }
                    writer2.Write((int)0); // <- fixture count
                    writer2.Write((int)(indexes.Count / 4));
                    writer2.Write((int)4); // <- stride
                    for (int i = 0; i < indexes.Count; ++i)
                    {
                        writer2.Write((int)indexes[i]);
                    }
                    WriteChunk(writer, ChunkType.Collision, ms);
                }

                GC.Collect();

                FixtureInfos.Clear();

                //Write Water info
                ms = new MemoryStream();
                using (BinaryWriter writer2 = new BinaryWriter(ms))
                {
                    writer2.Write(regionId); //region id
                    writer2.Write(zoneID);

                    List<float> vertices = new List<float>();
                    List<int> indexes = new List<int>();

                    foreach (WaterBody body in bodies)
                    {

                        Matrix startTransform = Matrix.Translation( //translation
                                -(float)body.X,
                                (float)body.Y,
                                -(float)body.Z
                            );
                        int baseIndex = vertices.Count / 3;

                        for (int i = 0; i < body.Vertices.Length; i += 3)
                        {
                            WarZoneLib.Vector3 result = startTransform.TransformPoint(new WarZoneLib.Vector3(body.Vertices[i], body.Vertices[i + 1], body.Vertices[i + 2]));
                            vertices.Add(result.X);
                            vertices.Add(result.Y);
                            vertices.Add(result.Z);
                        }


                        ushort uniqueID = 0;
                        ushort _zoneId = (ushort)zoneID;
                        byte instance = 0;

                        int data = (int)
                            (
                            (int)((uniqueID & 0xC000) << 16) |
                                (int)((_zoneId & 0x3FF) << 20) |
                                (int)((uniqueID & 0x3FFF) << 6) |
                                (int)(0x28 + instance)
                                );

                        for (int i = 0; i < body.Polygons.Length; i += 3)
                        {
                            indexes.Add(body.Polygons[i + 0] + baseIndex - 1);
                            indexes.Add(body.Polygons[i + 1] + baseIndex - 1);
                            indexes.Add(body.Polygons[i + 2] + baseIndex - 1);
                            indexes.Add(data);
                        }
                    }

                    writer2.Write((int)(vertices.Count / 3));

                    for (int i = 0; i < vertices.Count; ++i)
                    {
                        writer2.Write((float)vertices[i]);
                    }

                    writer2.Write(0); // <- fixture count
                    writer2.Write(indexes.Count / 4);
                    writer2.Write(4); // <- stride
                    for (int i = 0; i < indexes.Count; ++i)
                    {
                        writer2.Write((int)indexes[i]);
                    }

                    WriteChunk(writer, ChunkType.Water, ms);
                }
                GC.Collect();

                bodies.Clear();

            }
        }

        private static float Clamp(int value, float low, float high)
        {
            if (high < low)
            {
                float temp = high;
                high = low;// throw gcnew ArgumentException();
                low = temp;
            }
            if (value < low)
                return low;
            if (value > high)
                return high;
            return value;
        }

        private static void WriteChunk(BinaryWriter writer, ChunkType chunkType, MemoryStream stream)
        {
            writer.Write((int)chunkType);
            writer.Write((int)stream.Length);
            writer.Write(stream.GetBuffer(), 0, (int)stream.Length);
        }

        private static void WriteHeader(BinaryWriter writer)
        {
            writer.Write(new byte[] { 0x4F, 0x43, 0x43 });// chars 'O', 'C', 'C'
            writer.Write((byte)3); //file version
            writer.Write((byte)16); //header size
            writer.Write(new byte[11]);
        }

        private static void CleanUpTerrainFolder(string pathOut)
        {
            string[] dirs = Directory.GetDirectories(pathOut);
            foreach (var dir in dirs)
            {
                Console.WriteLine($"Cleaning {dir}...");
                if (Directory.Exists(Path.Combine(dir, "terrain")))
                {
                    Directory.Delete(Path.Combine(dir, "terrain"), true);
                }
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    if (file.Contains(".png") || file.Contains(".pcx") || file.Contains(".obj"))
                        File.Delete(file);
                }
            }
        }

        private static void PerformWater(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetDirectories(pathSource);
            int processed = 0;
            foreach (var dir in dirs)
            {
                ++processed;
                Console.WriteLine($"Converting: {dir}");
                SaveWater(dir, pathDestination);
                string num = Path.GetFileName(dir).Replace("zone", "");
                num = RemoveZeros(num);
                int zoneID = int.Parse(num);
                Console.WriteLine($"Converted zone #{zoneID}");
            }
        }

        private static readonly Random rand = new Random();
        private static void SaveWater(string path, string pathDestination)
        {
            List<WaterBody> bodies = new List<WaterBody>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(Path.Combine(path, "water.xml")));
            int i = 0;
            Bitmap bitmap = new Bitmap(1024, 1024);
            Graphics gr = Graphics.FromImage(bitmap);
            Brush brush = new SolidBrush(Color.FromArgb(rand.Next(256 / 32) * 32, rand.Next(256 / 32) * 32, rand.Next(256 / 32) * 32));
            Pen pen = new Pen(brush);
            foreach (XmlNode node in doc.GetElementsByTagName("WaterBody"))
            {
                float z = Convert.ToSingle(node.Attributes["height"].Value.Replace('.', ','));
                string name = node.Attributes["name"].Value;
                string type = node.Attributes["type"].Value;
                i++;

                List<Point> points = new List<Point>();
                List<Point> cpoints = new List<Point>();
                List<Point> cpointsWorld = new List<Point>();
                foreach (XmlNode fringe in node["Fringes"].ChildNodes)
                {
                    foreach (XmlNode cp in fringe["ControlPoints"].ChildNodes)
                    {
                        string pos = cp.Attributes.GetNamedItem("pos").Value;
                        float x = 1024f - Convert.ToSingle(pos.Split(' ')[0].Replace(',', ' ').Trim(' ').Replace('.', ',')) * 1023f / 65535.0f;
                        float y = Convert.ToSingle(pos.Split(' ')[1].Replace('.', ',')) * 1023f / 65535.0f;
                        points.Add(new Point((int)x, (int)y));
                    }
                }
                foreach (XmlNode fringe in node["ControlPoints"].ChildNodes)
                {
                    string pos = fringe.Attributes.GetNamedItem("left").Value;
                    float x_world = Convert.ToSingle(pos.Split(' ')[0].Replace(',', ' ').Trim(' ').Replace('.', ','));
                    float y_world = Convert.ToSingle(pos.Split(' ')[1].Replace('.', ','));
                    float x = x_world * 1023f / 65535.0f;
                    float y = y_world * 1023f / 65535.0f;
                    cpoints.Add(new Point((int)x, (int)y));
                    cpointsWorld.Add(new Point((int)x_world, (int)y_world));
                    pos = fringe.Attributes.GetNamedItem("right").Value;
                    x_world = Convert.ToSingle(pos.Split(' ')[0].Replace(',', ' ').Trim(' ').Replace('.', ','));
                    y_world = Convert.ToSingle(pos.Split(' ')[1].Replace('.', ','));
                    x = x_world * 1023f / 65535.0f;
                    y = y_world * 1023f / 65535.0f;
                    cpoints.Add(new Point((int)x, (int)y));
                    cpointsWorld.Add(new Point((int)x_world, (int)y_world));
                }

                foreach (Point p in points)
                {
                    gr.DrawRectangle(pen, new Rectangle(p, new Size(1, 1)));
                }

                float x_min = 65535f, y_min = 65535f;

                foreach (Point p in cpointsWorld)
                {
                    if (p.X < x_min)
                        x_min = p.X;
                    if (p.Y < y_min)
                        y_min = p.Y;
                }
                if (cpointsWorld.Count > 2)
                {
                    Structures.TriangleNet.Meshing.GenericMesher mesher = new Structures.TriangleNet.Meshing.GenericMesher();
                    Polygon poly = new Polygon(cpointsWorld.Count);
                    foreach (Point p in cpointsWorld)
                    {
                        poly.Add(new Vertex(65535f - (p.X - x_min), p.Y - y_min));
                    }
                    ConstraintOptions options = new ConstraintOptions();
                    options.ConformingDelaunay = false;
                    Mesh mesh = (Mesh)mesher.Triangulate(poly, options);
                    foreach (Edge edge in mesh.Edges)
                    {
                        Vertex v1 = mesh.Vertices.ElementAt(edge.P0);
                        Vertex v2 = mesh.Vertices.ElementAt(edge.P1);
                        gr.DrawLine(pen, new Point((int)((v1.X - x_min) * 1023f / 65535.0f), (int)((v1.Y + y_min) * 1023f / 65535.0f)), new Point((int)((v2.X - x_min) * 1023f / 65535.0f), (int)((v2.Y + y_min) * 1023f / 65535.0f)));
                    }
                    bodies.Add(new WaterBody(mesh, x_min, y_min, z, name, type));
                }
            }
            gr.Save();
            gr.Dispose();
            bitmap.Save(Path.Combine(pathDestination, Path.GetFileName(path), $"water.png"), ImageFormat.Png);
            bitmap.Dispose();
            if (bodies.Count > 0)
            {
                Console.WriteLine($"Saving {bodies.Count} water bodies...");
                using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathDestination, Path.GetFileName(path), $"water.bin"), FileMode.Create)))
                {
                    writer.Write((byte)0);
                    writer.Write(bodies.Count);
                    foreach (WaterBody body in bodies)
                    {
                        body.Serialize(writer);
                    }
                }
            }
        }

        private static void PerformNifToObj(string pathIn, string pathOut)
        {
            string[] dirs = Directory.GetFiles(pathIn);
            int processed = 0;
            foreach (var file in dirs)
            {
                if (!Path.GetExtension(file).Contains("nif"))
                {
                    continue;
                }
                processed++;
                string newFileName = Path.GetFileName(file).Replace(".nif", ".obj"); //remove fi.0.0.*
                /* if (File.Exists(Path.Combine(pathOut, newFileName)))
                 {
                     Console.WriteLine($"Exists: {file}, skipping");
                     Console.WriteLine(processed + "/" + dirs.Length);
                     continue;
                 }*/
                Console.WriteLine($"Converting: {file}");
                using (BinaryReader reader = new BinaryReader(new FileStream(file, FileMode.Open)))
                {
                    NiFile nif = new NiFile(reader);
                    IEnumerable<NiNode> nodes = NodeWalker.GetRoots(nif);
                    List<float> verts = new List<float>();
                    List<int> tris = new List<int>();
                    GathersCollision(ref verts, ref tris, nodes);
                    for (int i = tris.Count - 3; i > 0; i -= 3)
                    {
                        if (((tris[i] - 1) > verts.Count / 3) ||
                           ((tris[i + 1] - 1) > verts.Count / 3) ||
                           ((tris[i + 2] - 1) > verts.Count / 3))
                        {
                            tris.RemoveAt(i + 2);
                            tris.RemoveAt(i + 1);
                            tris.RemoveAt(i);
                        }
                    }
                    if (verts.Count > 0 && tris.Count > 0)
                    {
                        ObjModel model = new ObjModel(verts, tris);
                        model.SerializeObj(Path.Combine(pathOut, newFileName));
                    }
                }
                Console.WriteLine(processed + "/" + dirs.Length);
            }
        }

        private static void GathersCollision(ref List<float> verts, ref List<int> tris, IEnumerable<NiNode> nodes)
        {
            foreach (NiNode node in nodes)
            {
                GetCollisionInfo(node, ref verts, ref tris, false, 7);
            }
        }

        private static void GetCollisionInfo(NiNode node, ref List<float> verts, ref List<int> tris, bool inCollision, byte instanceId)
        {
            //Console.WriteLine($"Processing node: {GetNodeName(node)} ({node.GetType().Name})");
            if (!inCollision)
            {
                // We're in the collision subtree if this node is called 'collidee'
                inCollision = "collidee".Equals(GetNodeName(node));
            }
            else
            {
                //Nodes called 'nopick' can be present under a collidee node.
                // These nodes signify that everything below should not occlude LOS.
                inCollision = !"nopick".Equals(GetNodeName(node));
            }
            if ("door001".Equals(GetNodeName(node)))
            {
                instanceId = 0;
                inCollision = true;
            }
            else if ("door002".Equals(GetNodeName(node)))
            {
                instanceId = 1;
                inCollision = true;
            }
            else if ("door003".Equals(GetNodeName(node)))
            {
                instanceId = 2;
                inCollision = true;
            }
            else if ("door004".Equals(GetNodeName(node)))
            {
                instanceId = 3;
                inCollision = true;
            }
            else if ("door005".Equals(GetNodeName(node)))
            {
                instanceId = 4;
                inCollision = true;
            }
            else if ("door006".Equals(GetNodeName(node)))
            {
                instanceId = 5;
                inCollision = true;
            }
            if (inCollision)
            {

                // Console.WriteLine($"Getting triangles from: {GetNodeName(node)}");
                
                TriangleCollection col = TriangleWalker.GetTrianglesFromNode(node, false);
                int vertOffset = verts.Count;
                foreach (OpenTK.Vector3 vertex in col.Vertices)
                {
                    verts.Add(vertex.X); verts.Add(vertex.Y); verts.Add(vertex.Z);
                }

                foreach (TriangleIndex index in col.Indices)
                {
                    tris.Add((int)index.A + 1 + vertOffset);
                    tris.Add((int)index.B + 1 + vertOffset);
                    tris.Add((int)index.C + 1 + vertOffset);
                }

                //  Console.WriteLine($"Added: verts - {col.Vertices.Length}, tris - {col.Indices.Length}");
            }
            IEnumerable<NiAVObject> children = NodeWalker.GetChildren(node);
            foreach (NiAVObject child in children)
            {
                if (child is NiNode)
                {
                    GetCollisionInfo(child as NiNode, ref verts, ref tris, inCollision, instanceId);
                }
                else
                {
                    //    Console.WriteLine($"{child.Name} is not a node ({child.GetType()})");
                }
            }
        }
       
        private static object GetNodeName(NiNode node)
        {
            if (node != null && node.Name != null)
                return node.Name.Value.ToLower();

            return String.Empty;
        }

        private static void PerformSerialize(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetFiles(pathSource);
            int processed = 0;
            foreach (var file in dirs)
            {
                if (!Path.GetExtension(file).Contains("obj"))
                {
                    continue;
                }
                processed++;
                string newFileName = Path.GetFileName(file).Split('.')[3]; //remove fi.0.0.*

                Console.WriteLine($"Serializing: {file}");
                ObjModel model = new ObjModel();
                model.DeserializeObj(file);
                model.Serialize(Path.Combine(pathDestination, newFileName) + ".bin");
                Console.WriteLine(processed + "/" + dirs.Length);
            }
        }

        private static void PerformReverse(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetFiles(pathSource);
            int processed = 0;
            foreach (var file in dirs)
            {
                if (!Path.GetExtension(file).Contains("bin"))
                {
                    continue;
                }
                processed++;
                string newFileName = Path.GetFileName(file); //remove fi.0.0.*

                Console.WriteLine($"Serializing: {file}");
                ObjModel model = new ObjModel();
                model.Deserialize(file);
                model.SerializeObj(Path.Combine(pathDestination, newFileName) + ".obj");
                Console.WriteLine(processed + "/" + dirs.Length);
            }
        }

        private static void PerformSerializeTerrain(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetDirectories(pathSource);
            int processed = 0;
            foreach (var dir in dirs)
            {
                ++processed;
                Console.WriteLine($"Converting: {dir}");
                if (Directory.Exists(Path.Combine(dir, "terrain")))
                {
                    List<TerrainPiece> models = new List<TerrainPiece>();
                    foreach (string file in Directory.EnumerateFiles(Path.Combine(dir, "terrain")))
                    {
                        if (!file.Contains(".obj"))
                            continue;

                        string[] coords = Path.GetFileNameWithoutExtension(file).Replace("terrain", "").Split('_');
                        float x = Convert.ToInt32(coords[0]) * (65535f / 16f);
                        float y = Convert.ToInt32(coords[1]) * (65535f / 16f);
                        TerrainPiece model = new TerrainPiece(x, y);
                        model.DeserializeObj(file);
                        model.RemoveOffset();
                        model.Refine();
                        models.Add(model);
                    }
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathDestination, Path.GetFileName(dir), "terrain.bin"), FileMode.Create)))
                    {
                        writer.Write(models.Count);
                        foreach (TerrainPiece piece in models)
                        {
                            writer.Write(piece.XOffset);
                            writer.Write(piece.YOffset);
                            writer.Write(piece.Vertices.Length);
                            for (int i = 0; i < piece.Vertices.Length; ++i)
                            {
                                writer.Write(piece.Vertices[i]);
                            }
                            writer.Write(piece.Polygons.Length);
                            for (int i = 0; i < piece.Polygons.Length; ++i)
                            {
                                writer.Write(piece.Polygons[i]);
                            }
                        }
                    }
                }
                string num = Path.GetFileName(dir).Replace("zone", "");
                num = RemoveZeros(num);
                int zoneID = int.Parse(num);
                Console.WriteLine($"Converted zone terrain #{zoneID}");
            }
        }

        private static void PerformZones(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetDirectories(pathSource);
            int processed = 0;
            foreach (var dir in dirs)
            {
                ++processed;
                Console.WriteLine($"Converting: {dir}");
                SaveZone(dir, pathDestination);
                string num = Path.GetFileName(dir).Replace("zone", "");
                num = RemoveZeros(num);
                int zoneID = int.Parse(num);
                Console.WriteLine($"Converted zone #{zoneID}");
            }
        }

        private static void SaveZone(string path, string destination)
        {
            Dictionary<int, NifInfo> ModelInfos = new Dictionary<int, NifInfo>();
            Dictionary<int, FixtureInfo> FixtureInfos = new Dictionary<int, FixtureInfo>();
            int scaleFactor = 0;
            int offsetFactor = 0;
            byte[,] _OffsetData;
            byte[,] _TerrainData;
            ushort[,] HeightMap;
            bool[,] HoleMap;

            using (StreamReader reader = new StreamReader(new FileStream(Path.Combine(path, "sector.dat"), FileMode.Open)))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("scalefactor"))
                    {
                        scaleFactor = Convert.ToInt32(line.Split('=')[1]);
                    }
                    if (line.Contains("offsetfactor"))
                    {
                        offsetFactor = Convert.ToInt32(line.Split('=')[1]);
                    }
                }
            }

            Bitmap bitmap = new Bitmap(new FileStream(Path.Combine(path, "offset.png"), FileMode.Open));
            _OffsetData = new byte[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    _OffsetData[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
            bitmap.Dispose();

            bitmap = new Bitmap(new FileStream(Path.Combine(path, "terrain.png"), FileMode.Open));
            _TerrainData = new byte[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    _TerrainData[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
            bitmap.Dispose();
            if (!File.Exists(Path.Combine(path, "holemap.pcx")))
            {
                if (File.Exists(Path.Combine(path, "holemap.png")))
                {
                    File.Move(Path.Combine(path, "holemap.png"), Path.Combine(path, "holemap.pcx"));
                }
            }
            HoleMap = new bool[_TerrainData.GetLength(0), _TerrainData.GetLength(1)];

            for (int x = 0; x < HoleMap.GetLength(0); ++x)
            {
                for (int y = 0; y < HoleMap.GetLength(1); ++y)
                {
                    HoleMap[x, y] = false;
                }
            }

            if (File.Exists(Path.Combine(path, "holemap.pcx")))
            {
                FreeImageAPI.FreeImageBitmap fiBitmap = new FreeImageAPI.FreeImageBitmap(new FileStream(Path.Combine(path, "holemap.pcx"), FileMode.Open), FREE_IMAGE_FORMAT.FIF_PCX, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
                fiBitmap.Rescale(new Size(_TerrainData.GetLength(0), _TerrainData.GetLength(1)), FREE_IMAGE_FILTER.FILTER_BOX);
                bitmap = fiBitmap.ToBitmap();

                for (int x = 0; x < bitmap.Width; ++x)
                {
                    for (int y = 0; y < bitmap.Height; ++y)
                    {
                        byte pixel = bitmap.GetPixel(x, y).R;
                        if (pixel == 0)
                        {
                            HoleMap[x, y] = true;
                        }
                    }
                }
                bitmap.Dispose();
                fiBitmap.Dispose();
            }

            HeightMap = new ushort[_TerrainData.GetLength(0), _TerrainData.GetLength(1)];
            for (int x = 0; x < _TerrainData.GetLength(0); x++)
            {
                for (int y = 0; y < _TerrainData.GetLength(1); y++)
                {
                    HeightMap[x, y] = (ushort)(scaleFactor * _TerrainData[x, y] + offsetFactor * _OffsetData[x, y]);
                }
            }

            TerrainMesh tMesh = CreateTerrain(ref HeightMap, ref HoleMap);
            float[] verts = tMesh.BuildVertices();
            int[] inds = tMesh.BuildIndicies();
            ObjModel model = new ObjModel(verts, inds);
            model.SerializeObj(Path.Combine(destination, Path.GetFileName(path), "terrain.obj"));
            using (StreamReader reader = new StreamReader(Path.Combine(path, "nifs.csv")))
            {
                //Skip first two
                reader.ReadLine();
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] split = line.Split(',');
                    int id = int.Parse(split[0]);
                    string textureName = split[1];
                    string modelName = split[2];
                    bool only = int.Parse(split[3]) == 0 ? false : true;
                    bool shadow = int.Parse(split[4]) == 0 ? false : true;
                    int color = int.Parse(split[5]);
                    bool animate = int.Parse(split[6]) == 0 ? false : true;
                    bool collide = int.Parse(split[7]) == 0 ? false : true;
                    int ground = int.Parse(split[8]);
                    float mina = Convert.ToSingle(split[9]);
                    float maxa = Convert.ToSingle(split[10]);
                    if (!modelName.Contains("vfx_"))
                        ModelInfos.Add(id, new NifInfo(id, modelName.Replace(".nif", "").ToLower(), mina, maxa));
                }
            }
            using (StreamReader reader = new StreamReader(Path.Combine(path, "fixtures.csv")))
            {
                //Skip first two
                reader.ReadLine();
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string line = reader.ReadLine();
                        string[] split = line.Split(',');
                        int id = int.Parse(split[0]);
                        int nifId = int.Parse(split[1]);
                        string name = split[2];
                        double x = double.Parse(split[3].Replace(".", ","));
                        double y = double.Parse(split[4].Replace(".", ","));
                        double z = double.Parse(split[5].Replace(".", ","));
                        int o = int.Parse(split[6]);
                        int scale = int.Parse(split[7]);
                        int collide = int.Parse(split[8]);
                        int uID = int.Parse(split[14]);
                        double angle3D = double.Parse(split[15].Replace(".", ","));
                        double xAxis = double.Parse(split[16].Replace(".", ","));
                        double yAxis = double.Parse(split[17].Replace(".", ","));
                        double zAxis = double.Parse(split[18].Replace(".", ","));
                        if (!name.Contains("vfx_") && !name.Contains("tk_z_") && !name.Contains("sky_") && collide != 0)
                            FixtureInfos.Add(id, new FixtureInfo(id, (ushort)uID, (byte)7, nifId, name, x, y, z, o, scale, angle3D, xAxis, yAxis, zAxis));
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine("Error: " + x.ToString());
                    }
                }
            }
            if (!Directory.Exists(Path.Combine(destination, Path.GetFileName(path))))
            {
                Directory.CreateDirectory(Path.Combine(destination, Path.GetFileName(path)));
            }
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(destination, Path.GetFileName(path), "zoneInfo.bin"), FileMode.Create)))
            {
                writer.Write((byte)0); //version
                /* writer.Write(verts.Length);
                 for (int i2 = 0; i2 < verts.Length; ++i2)
                 {
                     writer.Write(verts[i2]);
                 }
                 writer.Write(inds.Length);
                 for (int i2 = 0; i2 < inds.Length; ++i2)
                 {
                     writer.Write(inds[i2]);
                 }*/
                writer.Write((int)ModelInfos.Count);
                foreach (int key in ModelInfos.Keys)
                {
                    writer.Write(StructToBytes<NifInfo>(ModelInfos[key]));
                }
                writer.Write((int)FixtureInfos.Count);
                foreach (int key in FixtureInfos.Keys)
                {
                    writer.Write(StructToBytes<FixtureInfo>(FixtureInfos[key]));
                }
            }
        }

        private static TerrainMesh CreateTerrain(ref ushort[,] heightMap, ref bool[,] holeMap)
        {
            int Height = heightMap.GetLength(0);
            int Width = heightMap.GetLength(1);
            TerrainMesh mesh = new TerrainMesh(Width, Height);
            int increment = 2;

            int vertId = 0;
            for (int x = 0; x < Width; x += increment)
            {
                for (int y = 0; y < Height; y += increment)
                {
                    float x0 = 0, y0 = 0, z0 = 0;
                    GetVertexPosition(x, y, ref x0, ref y0, ref z0, heightMap);
                    if (holeMap[1023 - x, y] != true)
                    {
                        mesh.HeightMap[x, y].ID = vertId++;
                        mesh.HeightMap[x, y].X = x0;
                        mesh.HeightMap[x, y].Y = y0;
                        mesh.HeightMap[x, y].Z = z0;
                        mesh.HeightMap[x, y].Type = VertexType.Valid;
                    }
                }
            }
            //build polygons
            for (int x = 0; x < Width - increment; x += increment)
            {
                for (int y = 0; y < Height - increment; y += increment)
                {
                    if (mesh.HeightMap[x, y].Type != VertexType.Hole)
                    {
                        if (mesh.HeightMap[x + increment, y].Type != VertexType.Hole &&
                            mesh.HeightMap[x, y + increment].Type != VertexType.Hole)
                        {
                            mesh.Polygons.Add(new TerrainPolygon(mesh.HeightMap[x, y], mesh.HeightMap[x + increment, y], mesh.HeightMap[x, y + increment]));
                        }
                    }
                    if (mesh.HeightMap[x, y + increment].Type != VertexType.Hole)
                    {
                        if (mesh.HeightMap[x + increment, y + increment].Type != VertexType.Hole &&
                            mesh.HeightMap[x + increment, y].Type != VertexType.Hole)
                        {
                            mesh.Polygons.Add(new TerrainPolygon(mesh.HeightMap[x, y + increment], mesh.HeightMap[x + increment, y], mesh.HeightMap[x + increment, y + increment]));
                        }
                    }
                }
            }
            return mesh;
        }
        /*
        private static float[] BuildVertices(ushort[,] HeightMap)
        {
            int Height = HeightMap.GetLength(0);
            int Width = HeightMap.GetLength(1);
            int numVerts = Width * Height;
            float[] arr = new float[numVerts * 3];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float x0 = 0, y0 = 0, z0 = 0;
                    GetVertexPosition(x, y, ref x0, ref y0, ref z0, HeightMap);

                    arr[x * 3 * Width + y * 3] = x0;
                    arr[x * 3 * Width + y * 3 + 1] = y0;
                    arr[x * 3 * Width + y * 3 + 2] = z0;
                }
            }
            return arr;
        }
                private static int[] BuildIndicies(ushort[,] HeightMap)
        {
            int Height = HeightMap.GetLength(0);
            int Width = HeightMap.GetLength(1);
            // FIXME: switch divide based on height delta
            int NumTriangles = 2 * (Width - 1) * (Height - 1);
            int[] indices = new int[3 * NumTriangles];
            int i = 0;
            for (int x = 0; x < Width - 1; x++)
            {
                for (int y = 0; y < Height - 1; y++)
                {
                    int b = x * Height; // base vertex index for this row

                    indices[i++] = b + y;
                    indices[i++] = b + y + Height;
                    indices[i++] = b + y + 1;

                    indices[i++] = b + y + 1;
                    indices[i++] = b + y + Height;
                    indices[i++] = b + y + Height + 1;
                }
            }
            return indices;
        }


        private static void CutHoles(ref float[] verts, ref int[] inds, ushort[] holeMap, ushort[,] HeightMap)
        {
            int Height = HeightMap.GetLength(0);
            int Width = HeightMap.GetLength(1);
            List<int> indexCopy = new List<int>(inds);
            List<float> vertexCopy = new List<float>(verts);
            for (int i = 0; i < holeMap.Length; i += 2)
            {
                ushort x = (ushort)(Height - holeMap[i]);
                ushort y = holeMap[i + 1];
                int holeIndex = x * Height + y;
                for (int i2 = indexCopy.Count - 3; i2 > 0; i2 -= 3)
                {
                    if (indexCopy[i2] == holeIndex ||
                       indexCopy[i2 + 1] == holeIndex ||
                       indexCopy[i2 + 2] == holeIndex)
                    {
                        indexCopy.RemoveAt(i2 + 2);
                        indexCopy.RemoveAt(i2 + 1);
                        indexCopy.RemoveAt(i2);
                    }
                }
            }
            verts = vertexCopy.ToArray();
            inds = indexCopy.ToArray();
        }
        */

        public static void GetVertexPosition(int x, int y, ref float x0, ref float y0, ref float z0, ushort[,] HeightMap)
        {
            int Height = HeightMap.GetLength(0);
            int Width = HeightMap.GetLength(1);
            x0 = 65535.0f * x / (float)(Width - 1);
            y0 = 65535.0f * y / (float)(Height - 1);
            z0 = (float)HeightMap[Width - x - 1, y];
        }


        private static byte[] StructToBytes<T>(T data) where T : struct
        {
            byte[] rawData = new byte[Marshal.SizeOf(data)];
            try
            {
                IntPtr rawDataPtr = Marshal.AllocHGlobal(rawData.Length);
                Marshal.StructureToPtr(data, rawDataPtr, false);
                Marshal.Copy(rawDataPtr, rawData, 0, rawData.Length);
                Marshal.FreeHGlobal(rawDataPtr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return rawData;
        }

        public static string RemoveZeros(string input)
        {
            int startIndex = -1;
            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] == '0')
                {
                    startIndex = i;
                }
                else
                {
                    break;
                }
            }
            return input.Substring(startIndex + 1);
        }

        private static void PerformSimplify(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetFiles(pathSource);
            int processed = 0;
            foreach (var file in dirs)
            {
                if (!Path.GetExtension(file).Contains("obj"))
                {
                    continue;
                }
                processed++;
                float epsilon = 0.01f;
                FileInfo info = new FileInfo(file);
                if (info.Length < 4000 * 1024)
                {
                    epsilon = 0.001f;
                }
                else if (info.Length < 10000 * 1024)
                {
                    epsilon = 0.0001f;
                }
                else if (info.Length < 15000 * 1024)
                {
                    epsilon = 0.00001f;
                }
                else if (info.Length < 20000 * 1024)
                {
                    epsilon = 0.000001f;
                }
                else
                {
                    epsilon = 0;
                }
                if (epsilon == 0)
                {
                    File.Copy(file, Path.Combine(pathDestination, Path.GetFileName(file)), true);
                }
                else
                {
                    Console.WriteLine($"Simplifying: {file}");
                    if (!File.Exists(Path.Combine(pathDestination, Path.GetFileName(file))))
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = "obj-simplify.exe";
                        proc.StartInfo.Arguments = $"-in \"{file}\" -out \"{Path.Combine(pathDestination, Path.GetFileName(file))}\" -workers 12 -gzip -1 -epsilon {epsilon.ToString().Replace(",", ".")}";
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.RedirectStandardError = true;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.EnableRaisingEvents = true;
                        proc.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
                        {
                            Console.WriteLine(e.Data);
                        });
                        proc.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
                        {
                            Console.WriteLine(e.Data);
                        });

                        proc.Start();
                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                        proc.WaitForExit();
                    }
                    else
                    {
                        Console.WriteLine("Already simplified, skipping!");
                    }
                }
                Console.WriteLine(processed + "/" + dirs.Length);
            }
        }

        private static void PerformPack(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetFiles(pathSource);
            int processed = 0;
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathDestination, "fixtures.bin"), FileMode.Create)))
            {
                writer.Write((byte)0); //version
                writer.Write((int)dirs.Length);
                foreach (var file in dirs)
                {
                    processed++;
                    Console.WriteLine($"Saving: {file}");
                    writer.Write((string)Path.GetFileNameWithoutExtension(file));
                    byte[] arr = File.ReadAllBytes(file);
                    writer.Write((int)arr.Length);
                    writer.Write(arr);
                    Console.WriteLine(processed + "/" + dirs.Length);
                }
            }
        }
    }
}
