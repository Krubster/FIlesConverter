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
    /// Class ATextureRenderData.
    /// </summary>
    public class ATextureRenderData : NiObject
	{
        /// <summary>
        /// The pixel format
        /// </summary>
        public ePixelFormat PixelFormat;

        /// <summary>
        /// The red mask
        /// </summary>
        public uint RedMask;

        /// <summary>
        /// The green mask
        /// </summary>
        public uint GreenMask;

        /// <summary>
        /// The blue mask
        /// </summary>
        public uint BlueMask;

        /// <summary>
        /// The alpha mask
        /// </summary>
        public uint AlphaMask;

        /// <summary>
        /// The bits per pixel
        /// </summary>
        public byte BitsPerPixel;

        /// <summary>
        /// The unkown3 bytes
        /// </summary>
        public byte[] Unkown3Bytes;

        /// <summary>
        /// The unkown8 bytes
        /// </summary>
        public byte[] Unkown8Bytes;

        /// <summary>
        /// The unkown int
        /// </summary>
        public uint UnkownInt;

        /// <summary>
        /// The unkown int2
        /// </summary>
        public uint UnkownInt2;

        /// <summary>
        /// The unkown int3
        /// </summary>
        public uint UnkownInt3;

        /// <summary>
        /// The unkown int4
        /// </summary>
        public uint UnkownInt4;

        /// <summary>
        /// The flags
        /// </summary>
        public byte Flags;

        /// <summary>
        /// The unkown byte1
        /// </summary>
        public byte UnkownByte1;

        /// <summary>
        /// The channel data
        /// </summary>
        public ChannelData[] ChannelData;

        /// <summary>
        /// The palette
        /// </summary>
        public NiRef<NiPalette> Palette;

        /// <summary>
        /// The number mip maps
        /// </summary>
        public uint NumMipMaps;

        /// <summary>
        /// The bytes per pixel
        /// </summary>
        public uint BytesPerPixel;

        /// <summary>
        /// The mip maps
        /// </summary>
        public MipMap[] MipMaps;

        /// <summary>
        /// Initializes a new instance of the <see cref="ATextureRenderData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public ATextureRenderData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			PixelFormat = (ePixelFormat)reader.ReadUInt32();
			if (Version <= eNifVersion.VER_10_2_0_0)
			{
				RedMask = reader.ReadUInt32();
				GreenMask = reader.ReadUInt32();
				BlueMask = reader.ReadUInt32();
				AlphaMask = reader.ReadUInt32();
				BitsPerPixel = reader.ReadByte();
				Unkown3Bytes = new byte[3];
				for (int i = 0; i < Unkown3Bytes.Length; i++)
				{
					Unkown3Bytes[i] = reader.ReadByte();
				}
				Unkown8Bytes = new byte[8];
				for (int j = 0; j < Unkown8Bytes.Length; j++)
				{
					Unkown8Bytes[j] = reader.ReadByte();
				}
			}
			if (Version >= eNifVersion.VER_10_0_1_0 && Version <= eNifVersion.VER_10_2_0_0)
			{
				UnkownInt = reader.ReadUInt32();
			}
			if (Version >= eNifVersion.VER_20_0_0_4)
			{
				BitsPerPixel = reader.ReadByte();
				UnkownInt2 = reader.ReadUInt32();
				UnkownInt3 = reader.ReadUInt32();
				Flags = reader.ReadByte();
				UnkownInt4 = reader.ReadUInt32();
			}
			if (Version >= eNifVersion.VER_20_3_0_6)
			{
				UnkownByte1 = reader.ReadByte();
			}
			if (Version >= eNifVersion.VER_20_0_0_4)
			{
				ChannelData = new ChannelData[4];
				for (int k = 0; k < 4; k++)
				{
					ChannelData[k] = new ChannelData(file, reader);
				}
			}
			Palette = new NiRef<NiPalette>(reader);
			NumMipMaps = reader.ReadUInt32();
			BytesPerPixel = reader.ReadUInt32();
			MipMaps = new MipMap[NumMipMaps];
			int num = 0;
			while ((long)num < (long)((ulong)NumMipMaps))
			{
				MipMaps[num] = new MipMap(file, reader);
				num++;
			}
		}
	}
}
