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
using FreeImageAPI;
using System.Xml;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using System.Text;
using WarZoneLib;
using CommonFConv;
using TriangleNet;
using TriangleNet.Meshing;
using TriangleNet.Geometry;
using System.Reflection;
using System.Collections;
using System.Xml.Linq;

namespace FIlesConverter
{
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
            bool packHeightmaps = false;
            bool genNavMesh = false;
            bool packCpp = false;
            bool doorless = false;
            bool hashes = false;
            string pathIn = "";
            string zonesFolder = ""; 
            string pathOut = "";
            var p = new OptionSet() {
                   { "in=",               "", v => pathIn = v },
                   { "out=",              "", v => pathOut = v  },
                   { "zonesFolder=",            "", v => zonesFolder = v  },
                   { "serialize",         "", v => serialize = v != null },
                   { "serializeTerrain",  "", v => serializeTerrain = v != null },
                   { "pack",              "", v => pack = v != null },
                   { "zones",             "", v => zones = v != null },
                   { "water",             "", v => water = v != null },
                   { "nifToObj",          "", v => nifToObj = v != null },
                   { "reverse",           "", v => reverse = v != null },
                   { "simplify",          "", v => simplify = v != null },
                   { "packWZ",            "", v => packWarZone = v != null },
                   { "packHeightmaps",    "", v => packHeightmaps = v != null },
                   { "genNavmesh",        "", v => genNavMesh = v != null },
                   { "packCpp",           "", v => packCpp = v != null },
                   { "doorless",          "", v => doorless = v != null },
                   { "hashes",            "", v => hashes = v != null },

          };
            List<string> extra = p.Parse(args);
            Console.WriteLine($"- Pack WarZone: {packWarZone}");
            Console.WriteLine($"- Pack: {pack}");
            Console.WriteLine($"- PackCpp: {packCpp}");
            Console.WriteLine($"- Serialize: {serialize}");
            Console.WriteLine($"- Zones: {zones}");
            Console.WriteLine($"- Simplify: {simplify}");
            Console.WriteLine($"- Serialize Terrain: {serializeTerrain}");
            Console.WriteLine($"- Reverse: {reverse}");
            Console.WriteLine($"- Nif-To-Obj: {nifToObj}");
            Console.WriteLine($"-- Doorless: {doorless}");
            Console.WriteLine($"- Water: {water}");
            Console.WriteLine($"- Pack Heightmaps: {packHeightmaps}");
            Console.WriteLine($"- Input: {pathIn}");
            Console.WriteLine($"- Output: {pathOut}");
            Console.WriteLine($"- GenNavmesh: {genNavMesh}");
            Console.WriteLine($"- Hashes: {hashes}");
            Console.WriteLine($"-- ZonesFolder: {zonesFolder}");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            try
            {
                if (!Directory.Exists(pathIn))
                {
                    Console.WriteLine("Wrong source path!");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }
                if (!Directory.Exists(pathOut))
                {
                    Console.WriteLine("Created output bin directory at: " + pathOut);
                    Directory.CreateDirectory(pathOut);
                }
                if (hashes)
                {
                    if (!Directory.Exists(zonesFolder))
                    {
                        Console.WriteLine("Wrong zones path!");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                        return;
                    }
                    PerformBruteforceHashes(pathIn, pathOut, zonesFolder);
                }
                if (packHeightmaps)
                {
                    PerformPackHeightmaps(pathIn, pathOut);
                }
                if (packWarZone)
                {
                    PerformPackWZ(pathIn, pathOut, doorless);
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
                    PerformNifToObj(pathIn, pathOut, doorless);
                }
                if (reverse)
                {
                    PerformReverse(pathIn, pathOut);
                }
                if (packCpp)
                {
                    PerformPackCpp(pathIn, pathOut);
                }
                if (pack)
                {
                    PerformPack(pathIn, pathOut, doorless ? "doorless_fixtures.bin" : "fixtures.bin");
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
                if (genNavMesh)
                {
                    PerformNavmeshGeneration(pathIn, pathOut);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("Done!");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void PerformBruteforceHashes(string pathIn, string pathOut, string zones)
        {
            Dictionary<string, string> nifs = new Dictionary<string, string>();
            Dictionary<long, string> hashes = new Dictionary<long, string>();
            Console.WriteLine("Reading nifs...");
            string[] dirs = Directory.GetDirectories(zones);
            foreach(string path in dirs)
            {
                Console.WriteLine($"Reading {path}");
                string fixturesConfig = Path.Combine(path, "fixtures.csv");
                if (!File.Exists(fixturesConfig)){
                    Console.WriteLine($"{path} doesn't have fixtures.csv file!");
                    continue;
                }
                using (StreamReader reader = new StreamReader(new FileStream(fixturesConfig, FileMode.Open)))
                {
                    reader.ReadLine();
                    reader.ReadLine();
                    string line = null;
                    while(!String.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        string[] splitLine = line.Split(',');
                        string nifNameRaw = splitLine[2].Split(' ')[0]; // second split to remove (Instance X)
                        string nifName = $"assetdb/fixtures/fi.0.0.{nifNameRaw}.nif";
                        if (!nifs.ContainsKey(nifName))
                        {
                            nifs.Add(nifName, $"fi.0.0.{nifNameRaw}.nif");
                        }
                    }
                }
            }
            Console.WriteLine($"Added {nifs.Count} nif entries");

            dirs = Directory.GetFiles(pathIn);
            int processed = 0;
            foreach (var dir in dirs)
            {
                ++processed;
                Console.WriteLine($"Processing: {dir}");
                string fileName = Path.GetFileNameWithoutExtension(dir).Split('_')[1]; //ignore crc part
                long hash = Convert.ToInt64(fileName.ToLower(), 16);
                if (!hashes.ContainsKey(hash)) {
                    hashes.Add(hash, dir);
                }
                else
                {
                    Console.WriteLine($"Duplicate hash: {dir}");
                }
            }
            Console.WriteLine($"Added {hashes.Count} hashes");
            Console.WriteLine($"Hashing...");
            foreach(long hash in hashes.Keys)
            {
                foreach(string nifName in nifs.Keys)
                {
                    if(hash == MYP.HashWAR(nifName))
                    {
                        File.Move(hashes[hash], Path.Combine(pathOut, nifs[nifName]));
                        break;
                    }
                }
            }
            Console.WriteLine($"Done!");
        }

        private static void PerformNavmeshGeneration(string pathIn, string pathOut)
        {
            string[] dirs = Directory.GetDirectories(pathIn);
            int processed = 0;
            Dictionary<string, ObjModel> models = new Dictionary<string, ObjModel>();
            if (!File.Exists(Path.Combine(@"fixtures_doorless.bin")))
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[OCCLUSION]: No fixtures_doorless.bin in root folder!");
                Console.ForegroundColor = oldColor;
                return;
            }
            using (BinaryReader reader = new BinaryReader(new FileStream(Path.Combine(@"fixtures_doorless.bin"), FileMode.Open)))
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

            foreach (var dir in dirs)
            {
                ++processed;
                Console.WriteLine($"Packing: {dir}");
                string num = Path.GetFileName(dir).Replace("zone", "");
                num = RemoveZeros(num);
                int zoneID = int.Parse(num);
                if (!File.Exists(Path.Combine(pathOut, $"nav_{zoneID}.bin")))
                {
                    GenNavMesh(dir, models);
                }
                Console.WriteLine($"Packed zone #{zoneID}");
            }
        }

        private static void GenNavMesh(string path, Dictionary<string, ObjModel> models)
        {
            string num = Path.GetFileName(path).Replace("zone", "");
            num = RemoveZeros(num);
            int zoneID = int.Parse(num);

            //Data to load
            Dictionary<int, CommonFConv.FixtureInfo> FixtureInfos = new Dictionary<int, CommonFConv.FixtureInfo>();
            Dictionary<int, NifInfo> ModelInfos = new Dictionary<int, NifInfo>();
            List<WaterBody> bodies = new List<WaterBody>();

            //Loading data
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

            TerrainMesh tMesh = Utils.CreateTerrain(ref HeightMap, ref HoleMap);

            float[] verts = tMesh.BuildVertices();
            List<WarZoneLib.Vector3> verts_out = new List<WarZoneLib.Vector3>(verts.Length / 3);
            int[] inds = tMesh.BuildIndicies();
            ObjModel tMeshObj = new ObjModel(verts, inds);
            //Rotating mesh
            Matrix startTransform = Matrix.Translation( //translation
                          0,
                          0,
                          -65535.0f
                      );

            Matrix rotMatrix = Matrix.RotationAxis(new WarZoneLib.Vector3(1, 0, 0), -(float)Math.PI / 2);
            // Matrix rot2D = Matrix.RotationZ(-(float)(Math.PI / 2));
            // rotMatrix = rot2D * rotMatrix * Matrix.RotationX(-(float)Math.PI / 2);
            Matrix scaleMatrix = Matrix.Scaling(1f, 1f, 1f);
            startTransform = scaleMatrix * rotMatrix * startTransform;

            for (int i = 0; i < tMeshObj.Vertices.Length; i += 3)
            {
                WarZoneLib.Vector3 result = startTransform.TransformPoint(new WarZoneLib.Vector3(tMeshObj.Vertices[i], tMeshObj.Vertices[i + 1], tMeshObj.Vertices[i + 2]));
                verts_out.Add(result);
            }

            tMeshObj = new ObjModel(verts_out, inds);
            //  tMeshObj.SerializeObj(Path.Combine(path, $"terrain_rotate.obj"));

            if (File.Exists(Path.Combine(path, "zoneInfo.bin"))) //new fast reading
            {
                using (BinaryReader reader = new BinaryReader(new FileStream(Path.Combine(path, "zoneInfo.bin"), FileMode.Open)))
                {
                    byte version = reader.ReadByte();

                    int count = reader.ReadInt32();
                    int structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(NifInfo));
                    byte[] arr = new byte[structSize];
                    for (int i = 0; i < count; ++i)
                    {
                        reader.Read(arr, 0, arr.Length);
                        NifInfo info = Utils.BytesToStruct<NifInfo>(arr);
                        ModelInfos.Add(info.ID, info);
                    }
                    count = reader.ReadInt32();
                    structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CommonFConv.FixtureInfo));
                    arr = new byte[structSize];
                    for (int i = 0; i < count; ++i)
                    {
                        reader.Read(arr, 0, arr.Length);
                        CommonFConv.FixtureInfo info = Utils.BytesToStruct<CommonFConv.FixtureInfo>(arr);
                        FixtureInfos.Add(info.ID, info);
                    }
                }
            }
            GC.Collect();

            /*if (File.Exists(Path.Combine(dir, "water.bin")))
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
            */

            Console.WriteLine($"- Added mesh for terrain");
            List<ObjModel> modelsObj = new List<ObjModel>();
            foreach (int key in FixtureInfos.Keys)
            {
                List<WarZoneLib.Vector3> verticesExport = new List<WarZoneLib.Vector3>();
                List<int> indexesExport = new List<int>();
                CommonFConv.FixtureInfo info = FixtureInfos[key];
                if (!ModelInfos.ContainsKey(info.NifID))
                {
                    Console.WriteLine($"Missing fixture info: {info.NifID} - {info.Name} in {path}");
                    continue;
                }
                if (!models.ContainsKey(ModelInfos[info.NifID].ModelName))
                {
                    //  Log.Debug("[OCCLUSION]", $"Missing model: {ModelInfos[info.NifID].ModelName}");
                    continue;
                }


                NifInfo nif = ModelInfos[info.NifID];
                ObjModel model = models[nif.ModelName];
                startTransform = Matrix.Translation( //translation
                        65535.0f - (float)info.X,
                        (float)info.Y,
                        (float)info.Z
                    );

                rotMatrix = Matrix.RotationAxis(new WarZoneLib.Vector3(-(float)info.XAxis, (float)info.YAxis, (float)info.ZAxis), (float)info.Angle3D);
                float angle2D = Utils.Clamp(info.O, nif.MinAngle, nif.MinAngle) / 180.0f * (float)Math.PI;
                Matrix rot2D = Matrix.RotationZ(angle2D);
                rotMatrix = rot2D * rotMatrix * Matrix.RotationZ((float)Math.PI);
                scaleMatrix = Matrix.Scaling(info.Scale / 100.0f, info.Scale / 100.0f, info.Scale / 100.0f);
                startTransform = scaleMatrix * rotMatrix * startTransform;

                for (int i = 0; i < model.Vertices.Length; i += 3)
                {
                    WarZoneLib.Vector3 result = startTransform.TransformPoint(new WarZoneLib.Vector3(model.Vertices[i], model.Vertices[i + 1], model.Vertices[i + 2]));
                    verticesExport.Add(result);
                }

                for (int i = 0; i < model.Polygons.Length; i += 3)
                {
                    indexesExport.Add(model.Polygons[i + 0]);
                    indexesExport.Add(model.Polygons[i + 1]);
                    indexesExport.Add(model.Polygons[i + 2]);
                }
                model = new ObjModel(verticesExport, indexesExport.ToArray());

                //Rotating it again...
                startTransform = Matrix.Translation( //translation
                      0,
                      0,
                      -65535.0f
                  );

                rotMatrix = Matrix.RotationAxis(new WarZoneLib.Vector3(1, 0, 0), -(float)Math.PI / 2);
                scaleMatrix = Matrix.Scaling(1f, 1f, 1f);
                startTransform = scaleMatrix * rotMatrix * startTransform;

                for (int i = 0; i < model.Vertices.Length; i += 3)
                {
                    WarZoneLib.Vector3 result = startTransform.TransformPoint(new WarZoneLib.Vector3(model.Vertices[i], model.Vertices[i + 1], model.Vertices[i + 2]));
                    verts_out.Add(result);
                }

                model = new ObjModel(verts_out, model.Polygons);
                modelsObj.Add(model);

                Console.WriteLine($"- Added mesh for: {info.UniqueID}, {nif.ModelName}");
            }
            Console.WriteLine($"- Generating mesh for navmesh....");
            // tMeshObj.SerializeObj(Path.Combine(path, $"nav.obj"));
            modelsObj.Add(tMeshObj);
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(path, $"{zoneID}_nav.bin"), FileMode.Create)))
            {
                foreach (ObjModel mod in modelsObj)
                    mod.Serialize(writer);
            }
        }

        private static void PerformPackHeightmaps(string pathIn, string pathOut)
        {
            string[] dirs = Directory.GetDirectories(pathIn);
            int processed = 0;
            Dictionary<int, int> Regions = new Dictionary<int, int>();
            Dictionary<int, string> Names = new Dictionary<int, string>();

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
                        line = reader.ReadLine();
                        bool regionFound = false;
                        bool nameFound = false;
                        while (!(line.Contains("region=") || line.Contains("name=")))
                        {
                            line = reader.ReadLine();
                            if (line.Contains("region"))
                            {
                                line = line.Split('=')[1];
                                int region = Convert.ToInt32(line);
                                if (!Regions.ContainsKey(zId))
                                {
                                    Regions.Add(zId, region);
                                }
                                regionFound = true;
                            }
                            else if (line.Contains("name"))
                            {
                                line = line.Split('=')[1];
                                string name = line;
                                if (!Names.ContainsKey(zId))
                                {
                                    Names.Add(zId, name);
                                }
                                nameFound = true;
                            }
                            if (nameFound && regionFound)
                                break;
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
                    if (Offsets.ContainsKey(zoneID))
                    {
                        PackTerrain(Names[zoneID], Regions[zoneID], Offsets[zoneID][0], Offsets[zoneID][1], dir, pathOut);
                    }
                    else
                    {
                        Console.WriteLine($"No offsets found for #{zoneID}");
                    }
                }
                Console.WriteLine($"Packed zone #{zoneID}");
            }
        }

        private static void PackTerrain(string zonename, int regionId, int xOff, int yOff, string dir, string pathOut)
        {
            string num = Path.GetFileName(dir).Replace("zone", "");
            num = RemoveZeros(num);
            int zoneID = int.Parse(num);

            //Data to load
            ushort[,] HeightMap;

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

            HeightMap = new ushort[_TerrainData.GetLength(0), _TerrainData.GetLength(1)];
            for (int x = 0; x < _TerrainData.GetLength(0); x++)
            {
                for (int y = 0; y < _TerrainData.GetLength(1); y++)
                {
                    HeightMap[x, y] = (ushort)(scaleFactor * _TerrainData[x, y] + offsetFactor * _OffsetData[x, y]);
                }
            }

            //Writing data
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathOut, $"{zoneID}.bin"), FileMode.Create)))
            {
                //Header
                WriteHeader(writer);

                //Write region info
                writer.Write(zonename);
                writer.Write(regionId); //region id
                writer.Write(zoneID);
                writer.Write(xOff << 12);
                writer.Write(yOff << 12);


                //Write Terrain info
                writer.Write(HeightMap.GetLength(0));
                writer.Write(HeightMap.GetLength(1));
                for (int i = 0; i < HeightMap.GetLength(0); ++i)
                {
                    for (int j = 0; j < HeightMap.GetLength(1); ++j)
                    {
                        writer.Write((ushort)HeightMap[i, j]);
                    }
                }
            }
        }

        //Dumb a little, searches output dir of all previous actions (requires zoneInfo.bin, water.bin, terrain.pcx, offset.pcx, holemap.pcx, sector.dat in every zone folder and fixtures.bin, zones.dat, zoneinfo.txt in root folder)
        private static void PerformPackWZ(string pathIn, string pathOut, bool doorless)
        {
            string[] dirs = Directory.GetDirectories(pathIn);
            int processed = 0;
            Dictionary<string, ObjModel> models = new Dictionary<string, ObjModel>();
            if (!File.Exists(Path.Combine(!doorless ? @"fixtures.bin" : "doorless_fixtures.bin")))
            {
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[OCCLUSION]: No fixtures.bin in root folder!");
                Console.ForegroundColor = oldColor;
                return;
            }
            using (BinaryReader reader = new BinaryReader(new FileStream(Path.Combine(!doorless ? @"fixtures.bin" : "doorless_fixtures.bin"), FileMode.Open)))
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

        private static void PackZoneOld(Dictionary<string, ObjModel> models, int regionId, int xOff, int yOff, string dir, string pathOut)
        {
            string num = Path.GetFileName(dir).Replace("zone", "");
            num = RemoveZeros(num);
            int zoneID = int.Parse(num);

            //Data to load
            Dictionary<int, CommonFConv.FixtureInfo> FixtureInfos = new Dictionary<int, CommonFConv.FixtureInfo>();
            Dictionary<int, NifInfo> ModelInfos = new Dictionary<int, NifInfo>();
            List<WaterBody> bodies = new List<WaterBody>();


            //Loading data
            int scaleFactor = 0;
            int offsetFactor = 0;
            byte[,] _OffsetData;
            byte[,] _TerrainData;
            ushort[,] HeightMap;
            bool[,] HoleMap;

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

            Bitmap bitmap = new Bitmap(new FileStream(Path.Combine(dir, "offset.png"), FileMode.Open));
            _OffsetData = new byte[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; ++x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    _OffsetData[x, y] = bitmap.GetPixel(x, y).R;
                }
            }
            bitmap.Dispose();

            bitmap = new Bitmap(new FileStream(Path.Combine(dir, "terrain.png"), FileMode.Open));
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
                FreeImageAPI.FreeImageBitmap fiBitmap = new FreeImageAPI.FreeImageBitmap(new FileStream(Path.Combine(dir, "holemap.pcx"), FileMode.Open), FREE_IMAGE_FORMAT.FIF_PCX, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
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
                        NifInfo info = Utils.BytesToStruct<NifInfo>(arr);
                        ModelInfos.Add(info.ID, info);
                    }
                    count = reader.ReadInt32();
                    structSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CommonFConv.FixtureInfo));
                    arr = new byte[structSize];
                    for (int i = 0; i < count; ++i)
                    {
                        reader.Read(arr, 0, arr.Length);
                        CommonFConv.FixtureInfo info = Utils.BytesToStruct<CommonFConv.FixtureInfo>(arr);
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
                TerrainMesh tMesh = Utils.CreateTerrain(ref HeightMap, ref HoleMap);
                float[] verts = tMesh.BuildVertices();
                int[] inds = tMesh.BuildIndicies();
                ObjModel tModel = new ObjModel(verts, inds);
                tModel.SerializeObj(Path.Combine(pathOut, $"{zoneID}_terrain.obj"));
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
                            if (HoleMap[j, i])
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
                        CommonFConv.FixtureInfo info = FixtureInfos[key];
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
                        float angle2D = Utils.Clamp(info.O, nif.MinAngle, nif.MinAngle) / 180.0f * (float)Math.PI;
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
                 //   model2.SerializeObj(Path.Combine(pathOut, $"{zoneID}_fixtures.obj"));
                    tModel.Append(model2);
                    tModel.SerializeObj(Path.Combine(pathOut, $"{zoneID}_nav.obj"));

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
                    GenericMesher mesher = new GenericMesher();
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

        private static void PerformNifToObj(string pathIn, string pathOut, bool doorless)
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
                    GathersCollision(ref verts, ref tris, nodes, doorless);
                    if (verts.Count > 0 && tris.Count > 0)
                    {
                        ObjModel model = new ObjModel(verts, tris);
                        model.SerializeObj(Path.Combine(pathOut, newFileName));
                    }
                }
                Console.WriteLine(processed + "/" + dirs.Length);
            }
        }

        private static void GathersCollision(ref List<float> verts, ref List<int> tris, IEnumerable<NiNode> nodes, bool doorless)
        {
            foreach (NiNode node in nodes)
            {
                GetCollisionInfo(node, ref verts, ref tris, false, 7, doorless);
            }
        }

        private static void GetCollisionInfo(NiNode node, ref List<float> verts, ref List<int> tris, bool inCollision, byte instanceId, bool doorless)
        {
            bool ignoreChildren = false;
            //Console.WriteLine($"Processing node: {GetNodeName(node)} ({node.GetType().Name})");
            if (!inCollision)
            {
                // We're in the collision subtree if this node is called 'collidee'
                inCollision = "collidee".Equals(((string)GetNodeName(node)).ToLower());
            }
            else
            {
                //Nodes called 'nopick' can be present under a collidee node.
                // These nodes signify that everything below should not occlude LOS.
                inCollision = !"nopick".Equals(((string)GetNodeName(node)).ToLower());
            }
            if (doorless && ((string)GetNodeName(node)).ToLower().Contains("door00"))
            {
                ignoreChildren = true;
            }
            if ("door001".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 0;
                inCollision = true;
            }
            else if ("door002".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 1;
                inCollision = true;
            }
            else if ("door003".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 2;
                inCollision = true;
            }
            else if ("door004".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 3;
                inCollision = true;
            }
            else if ("door005".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 4;
                inCollision = true;
            }
            else if ("door006".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 5;
                inCollision = true;
            }
            else if ("door007".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 6;
                inCollision = true;
            }
            else if ("door008".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 7;
                inCollision = true;
            }
            else if ("door009".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 8;
                inCollision = true;
            }
            else if ("door010".Equals(((string)GetNodeName(node)).ToLower()) && !doorless)
            {
                instanceId = 9;
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
                uint minVert = uint.MaxValue;
                foreach (TriangleIndex index in col.Indices)
                {
                    if (index.A > minVert)
                    {
                        minVert = index.A;
                    }
                    if (index.B > minVert)
                    {
                        minVert = index.B;
                    }
                    if (index.C > minVert)
                    {
                        minVert = index.C;
                    }
                }
                foreach (TriangleIndex index in col.Indices)
                {
                    tris.Add((int)((int)index.A + 1 + vertOffset / 3));
                    tris.Add((int)((int)index.B + 1 + vertOffset / 3));
                    tris.Add((int)((int)index.C + 1 + vertOffset / 3));
                }

                //  Console.WriteLine($"Added: verts - {col.Vertices.Length}, tris - {col.Indices.Length}");
            }
            if (!ignoreChildren)
            {
                IEnumerable<NiAVObject> children = NodeWalker.GetChildren(node);
                foreach (NiAVObject child in children)
                {
                    if (child is NiNode)
                    {
                        GetCollisionInfo(child as NiNode, ref verts, ref tris, inCollision, instanceId, doorless);
                    }
                    else
                    {
                        //    Console.WriteLine($"{child.Name} is not a node ({child.GetType()})");
                    }
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
            Dictionary<int, CommonFConv.FixtureInfo> FixtureInfos = new Dictionary<int, CommonFConv.FixtureInfo>();
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

            TerrainMesh tMesh = Utils.CreateTerrain(ref HeightMap, ref HoleMap);
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
                            FixtureInfos.Add(id, new CommonFConv.FixtureInfo(id, (ushort)uID, (byte)7, nifId, name, x, y, z, o, scale, angle3D, xAxis, yAxis, zAxis));
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
                    writer.Write(Utils.StructToBytes<NifInfo>(ModelInfos[key]));
                }
                writer.Write((int)FixtureInfos.Count);
                foreach (int key in FixtureInfos.Keys)
                {
                    writer.Write(Utils.StructToBytes<CommonFConv.FixtureInfo>(FixtureInfos[key]));
                }
            }
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

        private static void PerformPack(string pathSource, string pathDestination, string name = "fixtures.bin")
        {
            string[] dirs = Directory.GetFiles(pathSource);
            int processed = 0;
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathDestination, name), FileMode.Create)))
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

        private static void PerformPackCpp(string pathSource, string pathDestination)
        {
            string[] dirs = Directory.GetFiles(pathSource);
            int processed = 0;
            using (BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(pathDestination, "fixtures_cpp.bin"), FileMode.Create)))
            {
                writer.Write((byte)0); //version
                writer.Write((int)dirs.Length);
                foreach (var file in dirs)
                {
                    processed++;
                    Console.WriteLine($"Saving: {file}");
                    string filename = (string)Path.GetFileNameWithoutExtension(file);
                    writer.Write(filename.Length);
                    writer.Write(ASCIIEncoding.ASCII.GetBytes(filename));
                    byte[] arr = File.ReadAllBytes(file);
                    writer.Write((int)arr.Length);
                    writer.Write(arr);
                    Console.WriteLine(processed + "/" + dirs.Length);
                }
            }
        }
    }
}
