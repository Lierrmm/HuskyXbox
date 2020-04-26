using PhilLibX;
using PhilLibX.IO;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRPCLib;

namespace Husky
{
    class ModernWarfare2
    {
        public static XRPC console = new XRPC();
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct GfxMap
        {
            public int NamePointer { get; set; }
            public int MapNamePointer { get; set; }
            public int SurfaceCount { get; set; }
            public int GfxVertexCount { get; set; }
            public int GfxVerticesPointer { get; set; }
            public int GfxIndicesCount { get; set; }
            public int GfxIndicesPointer { get; set; }
            public int GfxStaticModelsCount { get; set; }
            public int GfxSurfacesPointer { get; set; }
            public int GfxStaticModelsPointer { get; set; }

            public static explicit operator GfxMap(uint value)
            {
                var p = new GfxMap
                {
                    NamePointer = console.ReadInt32(value),
                    MapNamePointer = console.ReadInt32(value + 0x4),
                    SurfaceCount = console.ReadInt32(0x83CAC77C),
                    GfxVertexCount = console.ReadInt32(0x83CAC5D0),
                    GfxVerticesPointer = console.ReadInt32(0x83CAC5D4),
                    GfxIndicesCount = console.ReadInt32(0x83CAC620),
                    GfxIndicesPointer = console.ReadInt32(0x83CAC624),
                    GfxStaticModelsCount = console.ReadInt32(0x83CAC778),
                    GfxSurfacesPointer = console.ReadInt32(0x83CAC7C8),
                    GfxStaticModelsPointer = console.ReadInt32(0x83CAC7D0)
                };
                return p;
            }
        }

        /// <summary>
        /// Gfx Map Surface
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct GfxSurface
        {
            /// <summary>
            /// Unknown Int (I know which pointer in the GfxMap it correlates it, but doesn't seem to interest us)
            /// </summary>
            public int UnknownBaseIndex { get; set; }

            /// <summary>
            /// Base Vertex Index (this is what allows the GfxMap to have 65k+ verts with only 2 byte indices)
            /// </summary>
            public int VertexIndex { get; set; }

            /// <summary>
            /// Number of Vertices this surface has
            /// </summary>
            public ushort VertexCount { get; set; }

            /// <summary>
            /// Number of Faces this surface has
            /// </summary>
            public ushort FaceCount { get; set; }

            /// <summary>
            /// Base Face Index (this is what allows the GfxMap to have 65k+ faces with only 2 byte indices)
            /// </summary>
            public int FaceIndex { get; set; }

            /// <summary>
            /// Pointer to the Material Asset of this Surface
            /// </summary>
            public int MaterialPointer { get; set; }

            /// <summary>
            /// Unknown Bytes
            /// </summary>
            public int crap { get; set; }

            public static explicit operator GfxSurface(uint value)
            {
                var p = new GfxSurface
                {
                    UnknownBaseIndex = console.ReadInt32(value),
                    VertexIndex = console.ReadInt32(value + 0x4),
                    VertexCount = console.ReadUInt16(value + 0x8),
                    FaceCount = console.ReadUInt16(value + 0xA),
                    FaceIndex = console.ReadInt32(value + 0xC),
                    MaterialPointer = console.ReadInt32(value + 0x10),
                    crap = console.ReadInt32(value + 0x14)
                };
                return p;
            }
        }

        /// <summary>
        /// Call of Duty: Modern Warfare 2 Material Asset
        /// </summary>
        public unsafe struct Material
        {
            /// <summary>
            /// A pointer to the name of this material
            /// </summary>
            public int NamePointer { get; set; }

            /// <summary>
            /// Unknown Bytes (Flags, settings, etc.)
            /// </summary>
            public fixed byte UnknownBytes[0x44];

            /// <summary>
            /// Number of Images this Material has
            /// </summary>
            public byte ImageCount { get; set; }

            /// <summary>
            /// Unknown Bytes (Flags, settings, etc.)
            /// </summary>
            public fixed byte UnknownBytes1[0xB];

            /// <summary>
            /// A pointer to this Material's Image table
            /// </summary>
            public int ImageTablePointer { get; set; }
            //TODO: Material Header
            public static explicit operator Material(uint value)
            {
                var p = new Material
                {
                    NamePointer = console.ReadInt32(value),
                    ImageCount = console.ReadByte(value + 0x44),
                    ImageTablePointer = console.ReadInt16(value + 0x47),
                };
                return p;
            }
        }


        /// <summary>
        /// Material Image for: MW2, MW3
        /// </summary>
        public unsafe struct MaterialImage32B
        {
            /// <summary>
            /// Semantic Hash/Usage
            /// </summary>
            public int SemanticHash { get; set; }

            /// <summary>
            /// Unknown Int
            /// </summary>
            public int UnknownInt { get; set; }

            /// <summary>
            /// Pointer to the Image Asset
            /// </summary>
            public int ImagePointer { get; set; }

            public static explicit operator MaterialImage32B(uint value)
            {
                var p = new MaterialImage32B
                {
                    SemanticHash = console.ReadInt32(value),
                    UnknownInt = console.ReadInt32(value + 0x4),
                    ImagePointer = console.ReadInt16(value + 0x8),
                };
                return p;
            }
        }

        /// <summary>
        /// Reads BSP Data
        /// </summary>
        public static void ExportBSPData(uint assetPoolsAddress, uint assetSizesAddress, string gameType,
            Action<object> printCallback = null)
        {
            console.Connect();
            if (!console.activeConnection)
            {
                printCallback?.Invoke("Couldn't connect");
                return;
            }

            // Found her
            printCallback?.Invoke("Found supported game: Call of Duty: Modern Warfare 2");
            // Validate by XModel Name
            printCallback?.Invoke($"AssetPools Address: {assetPoolsAddress:X}");
            //_xrpc
            var _void = console.ReadUInt32(console.ReadUInt32(assetPoolsAddress + 0x10) + 4);
            var txt = console.ReadString(_void);
            printCallback?.Invoke($"Address Found: {txt}");
            if (txt == "void")
            {
                var gfxMapAsset = (GfxMap)console.ReadUInt32(assetPoolsAddress + 0x4C);
                var gfxMapName = console.ReadString((uint)gfxMapAsset.NamePointer);
                var mapName = console.ReadString((uint)gfxMapAsset.MapNamePointer);
                if (string.IsNullOrWhiteSpace(gfxMapName))
                {
                    printCallback?.Invoke("No BSP loaded. Enter a Map to load in the required assets.");
                }
                else
                {
                    var mapFile = new IWMap();
                    printCallback?.Invoke($"Loaded Gfx Map     -   {gfxMapName}");
                    printCallback?.Invoke($"Loaded Map         -   {mapName}");
                    printCallback?.Invoke($"Vertex Count       -   {gfxMapAsset.GfxVertexCount}");
                    printCallback?.Invoke($"Indices Count      -   {gfxMapAsset.GfxIndicesCount}");
                    printCallback?.Invoke($"Surface Count      -   {gfxMapAsset.SurfaceCount}");
                    printCallback?.Invoke($"Model Count        -   {gfxMapAsset.GfxStaticModelsCount}");

                    string outputName = Path.Combine("exported_maps", "modern_warfare_2", gameType, mapName, mapName);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputName));
                    var stopWatch = Stopwatch.StartNew();
                    //Read vertex
                    printCallback?.Invoke("Parsing vertex data....");
                    var vertices = ReadGfxVertices(gfxMapAsset.GfxVerticesPointer, gfxMapAsset.GfxVertexCount);
                    printCallback?.Invoke($"Parsed vertex data in {stopWatch.ElapsedMilliseconds / 1000.0:0.00} seconds.");
                    // Reset timer
                    stopWatch.Restart();
                    //Read Indices
                    printCallback?.Invoke("Parsing surface indices....");
                    var indices = ReadGfxIndices(gfxMapAsset.GfxIndicesPointer, gfxMapAsset.GfxIndicesCount);
                    printCallback?.Invoke($"Parsed indices in {stopWatch.ElapsedMilliseconds / 1000.0:0.00} seconds.");
                    // Reset timer
                    stopWatch.Restart();

                    // Read Indices
                    printCallback?.Invoke("Parsing surfaces....");
                    var surfaces = ReadGfxSufaces(gfxMapAsset.GfxSurfacesPointer, gfxMapAsset.SurfaceCount);
                    printCallback?.Invoke($"Parsed surfaces in {stopWatch.ElapsedMilliseconds / 1000.0:0.00} seconds.");

                    // Reset timer
                    stopWatch.Restart();

                    // Write OBJ
                    printCallback?.Invoke("Converting to OBJ....");

                    // Create new OBJ
                    var obj = new WavefrontOBJ();

                    // Append Vertex Data
                    foreach (var vertex in vertices)
                    {
                        obj.Vertices.Add(vertex.Position);
                        obj.Normals.Add(vertex.Normal);
                        obj.UVs.Add(vertex.UV);
                    }


                    //Image Names (for Search String)
                    var imageNames = new HashSet<string>();

                    // Append Faces
                    foreach (var surface in surfaces)
                    {
                        // Create new Material
                        var material = ReadMaterial(surface.MaterialPointer);
                        // Add to images
                        imageNames.Add(material.DiffuseMap);
                        // Add it
                        obj.AddMaterial(material);
                        //Add points
                        for (ushort i = 0; i < surface.FaceCount; i++)
                        {
                            //Face Indices
                            var faceIndex1 = indices[i * 3 + surface.FaceIndex] + surface.VertexIndex;
                            var faceIndex2 = indices[i * 3 + surface.FaceIndex + 1] + surface.VertexIndex;
                            var faceIndex3 = indices[i * 3 + surface.FaceIndex + 2] + surface.VertexIndex;

                            //Validate unique points, and write to OBJ
                            if (faceIndex1 != faceIndex2 && faceIndex1 != faceIndex3 && faceIndex2 != faceIndex3)
                            {
                               //new Obj Face
                               var objFace = new WavefrontOBJ.Face(material.Name);

                                //Add points
                                objFace.Vertices[0] = new WavefrontOBJ.Face.Vertex(faceIndex1, faceIndex1, faceIndex1);
                                objFace.Vertices[2] = new WavefrontOBJ.Face.Vertex(faceIndex2, faceIndex2, faceIndex2);
                                objFace.Vertices[1] = new WavefrontOBJ.Face.Vertex(faceIndex3, faceIndex3, faceIndex3);

                                //Add to OBJ
                                obj.Faces.Add(objFace);
                            }
                        }
                    }

                    // Save it
                    obj.Save(outputName + ".obj");

                    //// Build search strinmg
                    //var searchString = imageNames.Aggregate("", (current, imageName) => current + $"{Path.GetFileNameWithoutExtension(imageName)},");

                    // Loop through images, and append each to the search string (for Wraith/Greyhound)

                    // Dump it
                    //File.WriteAllText(outputName + "_search_string.txt", searchString);

                    // Read entities and dump to map
                    mapFile.Entities.AddRange(ReadStaticModels((uint)gfxMapAsset.GfxStaticModelsPointer, gfxMapAsset.GfxStaticModelsCount));
                    mapFile.DumpToMap(outputName + ".map");

                    // Done
                    printCallback?.Invoke($"Converted to OBJ in {stopWatch.ElapsedMilliseconds / 1000.0:0.00} seconds.");
                }
            }
            else
            {
                printCallback?.Invoke("Call of Duty: Modern Warfare 2 is supported, but this EXE is not.");
            }
        }


        public static ushort[] ReadGfxIndices(int address, int count)
        {
            // Preallocate short array
            var indices = new ushort[count];
            // Read buffer
            var byteBuffer = console.GetMemory((uint)address, (uint)count * 2);
            // Copy buffer 
            Buffer.BlockCopy(byteBuffer, 0, indices, 0, byteBuffer.Length);
            // Done
            return indices;
        }

        public static Vertex[] ReadGfxVertices(int address, int count)
        { 
            // Preallocate vertex array
            var vertices = new Vertex[count];
            // Read buffer
            var byteBuffer = console.GetMemory((uint)address, (uint)count * 44);
            // Loop number of vertices we have
            for (var i = 0; i < count; i++)
            {
                // Read Struct
                var gfxVertex = ByteUtil.BytesToStruct<GfxVertex>(byteBuffer, i * 44);

                // Create new SEModel Vertex
                vertices[i] = new Vertex()
                {
                    // Set offset
                    Position = new Vector3(
                        gfxVertex.X * 2.54,
                        gfxVertex.Y * 2.54,
                        gfxVertex.Z * 2.54),
                    // Decode and set normal (from DTZxPorter - Wraith, same as XModels)
                    Normal = VertexNormalUnpacking.MethodA(gfxVertex.Normal),
                    // Set UV
                    UV = new Vector2(gfxVertex.U, 1 - gfxVertex.V)
                };
            }

            // Done
            return vertices;
        }


        public static GfxSurface[] ReadGfxSufaces(int address, int count)
        {
            // Preallocate short array
            var surfaces = new GfxSurface[count];

            // Loop number of indices we have
            for (var i = 0; i < count; i++)
            {
                //add it
                var _gfxDSurface = (GfxSurface)((uint)address + i * 24);
                surfaces[i] = _gfxDSurface;
            }

            // Done
            return surfaces;
        }

        ///TODO: Fix this crap
        /// <summary>
        /// Reads a material for the given surface and its associated images
        /// </summary>
        public static WavefrontOBJ.Material ReadMaterial(int address)
        {
            // Read Material
            var material = (Material)((uint)address);
            // Create new OBJ Image
            var _path = console.ReadString(console.ReadUInt32((uint)address)).Replace("*", "");
            var objMaterial = new WavefrontOBJ.Material(Path.GetFileNameWithoutExtension(_path));
            // Loop over images
            for (byte i = 0; i < material.ImageCount; i++)
            {
                // Read Material Image
                var materialImage =
                    (MaterialImage32B) ((uint) material.ImageTablePointer +
                                        i * Marshal
                                            .SizeOf<MaterialImage32B>()
                    );
                // Check for color map for now
                if (materialImage.SemanticHash != 0xA0AB1041) continue;
                var str = console.ReadString((uint) materialImage.ImagePointer + 0x1C) + ".png";
                objMaterial.DiffuseMap = "_images\\\\" + str;
            }

            // Done
            return objMaterial;
        }

        ///TODO: Fix this crap
        public static unsafe List<IWMap.Entity> ReadStaticModels(uint address, int count)
        {
            // Resulting Entities
            var entities = new List<IWMap.Entity>(count);
            // Read buffer
            var byteBuffer = console.GetMemory(address, (uint)(count * Marshal.SizeOf<GfxStaticModel>()));
            // Loop number of models we have
            for (var i = 0; i < count; i++)
            {
                // Read Struct
                var staticModel = ByteUtil.BytesToStruct<GfxStaticModel>(byteBuffer, i * Marshal.SizeOf<GfxStaticModel>());
                // Model Name
                var modelName = console.ReadString((uint)staticModel.ModelPointer);//reader.ReadNullTerminatedString(reader.ReadInt32(staticModel.ModelPointer));
                // New Matrix
                var matrix = new Rotation.Matrix
                {
                    Values =
                    {
                        [0] = staticModel.Matrix[0],
                        [1] = staticModel.Matrix[1],
                        [2] = staticModel.Matrix[2],
                        [4] = staticModel.Matrix[3],
                        [5] = staticModel.Matrix[4],
                        [6] = staticModel.Matrix[5],
                        [8] = staticModel.Matrix[6],
                        [9] = staticModel.Matrix[7],
                        [10] = staticModel.Matrix[8]
                    }
                };
                // Copy X Values
                // Copy Y Values
                // Copy Z Values
                // Convert to Euler
                var euler = matrix.ToEuler();
                // Add it
                entities.Add(IWMap.Entity.CreateMiscModel(modelName, new Vector3(staticModel.X, staticModel.Y, staticModel.Z), Rotation.ToDegrees(euler), staticModel.ModelScale));
            }
            // Done
            return entities;
        }
    }
}
