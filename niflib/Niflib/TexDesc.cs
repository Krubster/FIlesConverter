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
	#if OpenTK
	using OpenTK;
	#elif SharpDX
	using SharpDX;
	#elif MonoGame
	using Microsoft.Xna.Framework;
	#endif
	using System;
	using System.IO;

    /// <summary>
    /// Class TexDesc.
    /// </summary>
    public class TexDesc
	{
        /// <summary>
        /// The source
        /// </summary>
        public NiRef<NiSourceTexture> Source;

        /// <summary>
        /// The clamp mode
        /// </summary>
        public eTexClampMode ClampMode;

        /// <summary>
        /// The filter mode
        /// </summary>
        public eTexFilterMode FilterMode;

        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The uv set index
        /// </summary>
        public uint UVSetIndex;

        /// <summary>
        /// The p s2 l
        /// </summary>
        public short PS2L;

        /// <summary>
        /// The p s2 k
        /// </summary>
        public short PS2K;

        /// <summary>
        /// The has texture transform
        /// </summary>
        public bool HasTextureTransform;

        /// <summary>
        /// The translation
        /// </summary>
        public Vector2 Translation;

        /// <summary>
        /// The tiling
        /// </summary>
        public Vector2 Tiling;

        /// <summary>
        /// The w rotation
        /// </summary>
        public float WRotation;

        /// <summary>
        /// The transform type
        /// </summary>
        public uint TransformType;

        /// <summary>
        /// The center offset
        /// </summary>
        public Vector2 CenterOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="TexDesc"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public TexDesc(NiFile file, BinaryReader reader)
		{
			Source = new NiRef<NiSourceTexture>(reader);
			if ( (int)file.Version <= 0x14000005 )
			{
				ClampMode = (eTexClampMode)reader.ReadUInt32();
				FilterMode = (eTexFilterMode)reader.ReadUInt32();
			}
			if ( (int)file.Version >= 0x14010003 )
			{
				Flags = reader.ReadUInt16();
			}
            if ( (int)file.Version >= 0x14060000 )
			{
				reader.ReadUInt16();
			}
			if ( (int)file.Version <= 0x14000005 )
			{
				UVSetIndex = reader.ReadUInt32();
			}
			if ( (int)file.Version <= 0x0A040001 )
			{
				PS2L = reader.ReadInt16();
				PS2K = reader.ReadInt16();
			}
			if ( (int)file.Version <= 0x0401000C )
			{
				reader.ReadUInt16();
			}
			if ( (int)file.Version >= 0x0A010000 )
			{
				HasTextureTransform = reader.ReadBoolean(file.Version);
				if (HasTextureTransform)
				{
					Translation = reader.ReadVector2();
					Tiling = reader.ReadVector2();
					WRotation = reader.ReadSingle();
					TransformType = reader.ReadUInt32();
					CenterOffset = reader.ReadVector2();
				}
			}
		}
	}
}
