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
    /// Class NiPixelData.
    /// </summary>
    public class NiPixelData : ATextureRenderData
	{
        /// <summary>
        /// The number pixels
        /// </summary>
        public uint NumPixels;

        /// <summary>
        /// The number faces
        /// </summary>
        public uint NumFaces;

        /// <summary>
        /// The pixel data
        /// </summary>
        public byte[][] PixelData;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPixelData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPixelData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			NumPixels = reader.ReadUInt32();
			if (Version >= eNifVersion.VER_20_0_0_4)
			{
				NumFaces = reader.ReadUInt32();
				PixelData = new byte[NumFaces][];
				int num = 0;
				while ((long)num < (long)((ulong)NumFaces))
				{
					PixelData[num] = new byte[NumPixels];
					int num2 = 0;
					while ((long)num2 < (long)((ulong)NumPixels))
					{
						PixelData[num][num2] = reader.ReadByte();
						num2++;
					}
					num++;
				}
			}
			if (Version <= eNifVersion.VER_10_2_0_0)
			{
				NumFaces = 1u;
				PixelData = new byte[NumFaces][];
				int num3 = 0;
				while ((long)num3 < (long)((ulong)NumFaces))
				{
					PixelData[num3] = new byte[NumPixels];
					int num4 = 0;
					while ((long)num4 < (long)((ulong)NumPixels))
					{
						PixelData[num3][num4] = reader.ReadByte();
						num4++;
					}
					num3++;
				}
			}
		}
	}
}
