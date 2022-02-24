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
    /// Class NiGeometry.
    /// </summary>
    public class NiGeometry : NiAVObject
	{
        /// <summary>
        /// The data
        /// </summary>
        public NiRef<NiGeometryData> Data;

        /// <summary>
        /// The skin instance
        /// </summary>
        public NiRef<NiSkinInstance> SkinInstance;

        /// <summary>
        /// The material names
        /// </summary>
        public NiString[] MaterialNames;

        /// <summary>
        /// The material extra data
        /// </summary>
        public int[] MaterialExtraData;

        /// <summary>
        /// The active material
        /// </summary>
        public int ActiveMaterial;

        /// <summary>
        /// The has shader
        /// </summary>
        public bool HasShader;

        /// <summary>
        /// The shader name
        /// </summary>
        public string ShaderName;

        /// <summary>
        /// The unkown integer
        /// </summary>
        public uint UnkownInteger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiGeometry"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">unsupported data</exception>
        public NiGeometry(NiFile file, BinaryReader reader) : base(file, reader)
		{
			Data = new NiRef<NiGeometryData>(reader);
			if (Version >= eNifVersion.VER_3_3_0_13)
			{
				SkinInstance = new NiRef<NiSkinInstance>(reader);
			}
			if (Version >= eNifVersion.VER_20_2_0_7)
			{
				MaterialNames = new NiString[reader.ReadUInt32()];
				for (int i = 0; i < MaterialNames.Length; i++)
				{
					MaterialNames[i] = new NiString(file, reader);
				}
				MaterialExtraData = new int[MaterialNames.Length];
				for (int j = 0; j < MaterialNames.Length; j++)
				{
					MaterialExtraData[j] = reader.ReadInt32();
				}
				ActiveMaterial = reader.ReadInt32();
			}
			if (Version >= eNifVersion.VER_10_0_1_0 && Version <= eNifVersion.VER_20_1_0_3)
			{
				HasShader = reader.ReadBoolean(Version);
				if (HasShader)
				{
					ShaderName = new NiString(file, reader).Value;
					UnkownInteger = reader.ReadUInt32();
				}
			}
			if (Version == eNifVersion.VER_10_4_0_1)
			{
				reader.ReadUInt32();
			}
			if (Version >= eNifVersion.VER_20_2_0_7)
			{
				reader.ReadBoolean();
			}
		}
	}
}
