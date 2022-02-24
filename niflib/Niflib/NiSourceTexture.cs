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
    /// Class NiSourceTexture.
    /// </summary>
    public class NiSourceTexture : NiTexture
	{
        /// <summary>
        /// The use external
        /// </summary>
        public bool UseExternal;

        /// <summary>
        /// The file name
        /// </summary>
        public NiString FileName;

        /// <summary>
        /// The pixel layout
        /// </summary>
        public ePixelLayout PixelLayout;

        /// <summary>
        /// The use mipmaps
        /// </summary>
        public eMipMapFormat UseMipmaps;

        /// <summary>
        /// The alpha format
        /// </summary>
        public eAlphaFormat AlphaFormat;

        /// <summary>
        /// The is static
        /// </summary>
        public bool IsStatic;

        /// <summary>
        /// The direct render
        /// </summary>
        public bool DirectRender;

        /// <summary>
        /// The persistent render data
        /// </summary>
        public bool PersistentRenderData;

        /// <summary>
        /// The internal texture
        /// </summary>
        public NiRef<ATextureRenderData> InternalTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiSourceTexture" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiSourceTexture(NiFile file, BinaryReader reader) : base(file, reader)
		{
			IsStatic = true;
			UseExternal = reader.ReadBoolean(Version);
			if (UseExternal)
			{
				FileName = new NiString(file, reader);
				if (Version >= eNifVersion.VER_10_1_0_0)
				{
					reader.ReadUInt32();
				}
			}
			if (!UseExternal)
			{
				if (Version <= eNifVersion.VER_10_0_1_0)
				{
					reader.ReadByte();
				}
				if (Version >= eNifVersion.VER_10_1_0_0)
				{
					FileName = new NiString(file, reader);
				}
				InternalTexture = new NiRef<ATextureRenderData>(reader);
			}
			PixelLayout = (ePixelLayout)reader.ReadUInt32();
			UseMipmaps = (eMipMapFormat)reader.ReadUInt32();
			AlphaFormat = (eAlphaFormat)reader.ReadUInt32();
			IsStatic = reader.ReadBoolean(Version);
			if (Version >= eNifVersion.VER_10_1_0_106)
			{
				DirectRender = reader.ReadBoolean(Version);
			}
			if (Version >= eNifVersion.VER_20_2_0_7)
			{
				PersistentRenderData = reader.ReadBoolean(Version);
			}
		}
	}
}
