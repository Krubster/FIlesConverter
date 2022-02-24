/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

namespace Niflib
{
	using System;
	using System.IO;

    /// <summary>
    /// Class SkinPartition.
    /// </summary>
    public class SkinPartition
	{
        /// <summary>
        /// The number vertices
        /// </summary>
        public ushort NumVertices;

        /// <summary>
        /// The number triangles
        /// </summary>
        public ushort NumTriangles;

        /// <summary>
        /// The number bones
        /// </summary>
        public ushort NumBones;

        /// <summary>
        /// The number strips
        /// </summary>
        public ushort NumStrips;

        /// <summary>
        /// The number weights per vertex
        /// </summary>
        public ushort NumWeightsPerVertex;

        /// <summary>
        /// The bones
        /// </summary>
        public ushort[] Bones;

        /// <summary>
        /// The has vertex map
        /// </summary>
        public bool HasVertexMap;

        /// <summary>
        /// The vertex map
        /// </summary>
        public ushort[] VertexMap;

        /// <summary>
        /// The has vertex weights
        /// </summary>
        public bool HasVertexWeights;

        /// <summary>
        /// The vertex weights
        /// </summary>
        public float[][] VertexWeights;

        /// <summary>
        /// The strip lengths
        /// </summary>
        public ushort[] StripLengths;

        /// <summary>
        /// The has faces
        /// </summary>
        public bool HasFaces;

        /// <summary>
        /// The strips
        /// </summary>
        public ushort[][] Strips;

        /// <summary>
        /// The triangles
        /// </summary>
        public Triangle[] Triangles;

        /// <summary>
        /// The has bone indicies
        /// </summary>
        public bool HasBoneIndicies;

        /// <summary>
        /// The bone indicies
        /// </summary>
        public byte[][] BoneIndicies;

        /// <summary>
        /// The unkown short
        /// </summary>
        public ushort UnkownShort;

        /// <summary>
        /// The unkown short2
        /// </summary>
        public ushort UnkownShort2;

        /// <summary>
        /// The unkown short3
        /// </summary>
        public ushort UnkownShort3;

        /// <summary>
        /// The number vertices2
        /// </summary>
        public ushort NumVertices2;

        /// <summary>
        /// The unkown short4
        /// </summary>
        public ushort UnkownShort4;

        /// <summary>
        /// The unkown short5
        /// </summary>
        public ushort UnkownShort5;

        /// <summary>
        /// The unkown short6
        /// </summary>
        public ushort UnkownShort6;

        /// <summary>
        /// The unkown arr
        /// </summary>
        public SkinPartitionUnkownItem1[] UnkownArr;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinPartition"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public SkinPartition(NiFile file, BinaryReader reader)
		{
			NumVertices = reader.ReadUInt16();
			NumTriangles = reader.ReadUInt16();
			NumBones = reader.ReadUInt16();
			NumStrips = reader.ReadUInt16();
			NumWeightsPerVertex = reader.ReadUInt16();
			Bones = reader.ReadUInt16Array((int)NumBones);
			HasVertexMap = true;
			HasVertexWeights = true;
			HasFaces = true;
			if (file.Version >= eNifVersion.VER_10_1_0_0)
			{
				HasVertexMap = reader.ReadBoolean(file.Version);
			}
			if (HasVertexMap)
			{
				VertexMap = reader.ReadUInt16Array((int)NumVertices);
			}
			if (file.Version >= eNifVersion.VER_10_1_0_0)
			{
				HasVertexWeights = reader.ReadBoolean(file.Version);
			}
			if (HasVertexWeights)
			{
				VertexWeights = new float[(int)NumVertices][];
				for (int i = 0; i < (int)NumVertices; i++)
				{
					VertexWeights[i] = reader.ReadFloatArray((int)NumWeightsPerVertex);
				}
			}
			StripLengths = reader.ReadUInt16Array((int)NumStrips);
			if (file.Version >= eNifVersion.VER_10_1_0_0)
			{
				HasFaces = reader.ReadBoolean(file.Version);
			}
			if (HasFaces && NumStrips != 0)
			{
				Strips = new ushort[(int)NumStrips][];
				for (int j = 0; j < (int)NumStrips; j++)
				{
					Strips[j] = reader.ReadUInt16Array((int)StripLengths[j]);
				}
			}
			else if (HasFaces && NumStrips == 0)
			{
				Triangles = new Triangle[(int)NumTriangles];
				for (int k = 0; k < Triangles.Length; k++)
				{
					Triangles[k] = new Triangle(reader);
				}
			}
			HasBoneIndicies = reader.ReadBoolean(file.Version);
			if (HasBoneIndicies)
			{
				BoneIndicies = new byte[(int)NumVertices][];
				for (int l = 0; l < BoneIndicies.Length; l++)
				{
					BoneIndicies[l] = new byte[(int)NumWeightsPerVertex];
					for (int m = 0; m < (int)NumWeightsPerVertex; m++)
					{
						BoneIndicies[l][m] = reader.ReadByte();
					}
				}
			}
			if (file.Header.UserVersion >= 12u)
			{
				UnkownShort = reader.ReadUInt16();
			}
			if (file.Version == eNifVersion.VER_10_2_0_0 && file.Header.UserVersion == 1u)
			{
				UnkownShort2 = reader.ReadUInt16();
				UnkownShort3 = reader.ReadUInt16();
				NumVertices2 = reader.ReadUInt16();
				UnkownShort4 = reader.ReadUInt16();
				UnkownShort5 = reader.ReadUInt16();
				UnkownShort6 = reader.ReadUInt16();
				UnkownArr = new SkinPartitionUnkownItem1[(int)NumVertices2];
				for (int n = 0; n < (int)NumVertices2; n++)
				{
					UnkownArr[n] = new SkinPartitionUnkownItem1(file, reader);
				}
			}
		}
	}
}
