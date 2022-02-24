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
    /// Class NiTexturingProperty.
    /// </summary>
    public class NiTexturingProperty : NiProperty
    {
        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The apply mode
        /// </summary>
        public uint ApplyMode;

        /// <summary>
        /// The texture count
        /// </summary>
        public uint TextureCount;

        /// <summary>
        /// The base texture
        /// </summary>
        public TexDesc BaseTexture;

        /// <summary>
        /// The dark texture
        /// </summary>
        public TexDesc DarkTexture;

        /// <summary>
        /// The detail texture
        /// </summary>
        public TexDesc DetailTexture;

        /// <summary>
        /// The gloss texture
        /// </summary>
        public TexDesc GlossTexture;

        /// <summary>
        /// The glow texture
        /// </summary>
        public TexDesc GlowTexture;

        /// <summary>
        /// The bump map texture
        /// </summary>
        public TexDesc BumpMapTexture;

        /// <summary>
        /// The decal0 texture
        /// </summary>
        public TexDesc Decal0Texture;

        /// <summary>
        /// The decal1 texture
        /// </summary>
        public TexDesc Decal1Texture;

        /// <summary>
        /// The decal2 texture
        /// </summary>
        public TexDesc Decal2Texture;

        /// <summary>
        /// The decal3 texture
        /// </summary>
        public TexDesc Decal3Texture;

        /// <summary>
        /// The normal texture
        /// </summary>
        public TexDesc NormalTexture;

        /// <summary>
        /// The unkown1
        /// </summary>
        public uint Unkown1;

        /// <summary>
        /// The bump map luma scale
        /// </summary>
        public float BumpMapLumaScale;

        /// <summary>
        /// The bump map luma offset
        /// </summary>
        public float BumpMapLumaOffset;

        /// <summary>
        /// The bump map matrix
        /// </summary>
        public Vector3 BumpMapMatrix;

        /// <summary>
        /// The number shader textures
        /// </summary>
        public uint NumShaderTextures;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiTexturingProperty"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTexturingProperty(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if ( (int)Version <= 0x0A000102 || (int)Version >= 0x14010003 )
            {
                Flags = reader.ReadUInt16();
            }
            if ( (int)Version <= 0x14000005 )
            {
                ApplyMode = reader.ReadUInt32();
            }
            TextureCount = reader.ReadUInt32();
            if (reader.ReadBoolean(Version))
            {
                BaseTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                DarkTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                DetailTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                GlossTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                GlowTexture = new TexDesc(file, reader);
            }
            if (reader.ReadBoolean(Version))
            {
                BumpMapTexture = new TexDesc(file, reader);
                BumpMapLumaScale = reader.ReadSingle();
                BumpMapLumaOffset = reader.ReadSingle();
                BumpMapMatrix = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                reader.ReadSingle();
            }
            if ((int)Version >= 0x14020007)
            {
                if (reader.ReadBoolean(Version))
                {
                    NormalTexture = new TexDesc(file, reader);
                }
                if (reader.ReadBoolean(Version)) //unknown
                {
                    new TexDesc(file, reader);
                    reader.ReadSingle();
                }
            }
            if (reader.ReadBoolean(Version))
            {
                Decal0Texture = new TexDesc(file, reader);
            }
            bool hasDecal1Texture = false;
            if ((int)Version <= 0x14010003)
            {
                if ((TextureCount >= 8))
                {
                    hasDecal1Texture = reader.ReadBoolean();
                }
            }
            if ((int)Version >= 0x14020007)
            {
                if ((TextureCount >= 10))
                {
                    hasDecal1Texture = reader.ReadBoolean();
                }
            }
            if (hasDecal1Texture)
            {
                Decal1Texture = new TexDesc(file, reader);
            }
            bool hasDecal2Texture = false;
            if ((int)Version <= 0x14010003)
            {
                if ((TextureCount >= 9))
                {
                    hasDecal2Texture = reader.ReadBoolean();
                }
            }
            if ((int)Version >= 0x14020007)
            {
                if ((TextureCount >= 11))
                {
                    hasDecal2Texture = reader.ReadBoolean();
                }
            }
            if (hasDecal2Texture)
            {
                Decal2Texture = new TexDesc(file, reader);
            }
            bool hasDecal3Texture = false;
            if ((int)Version <= 0x14010003)
            {
                if ((TextureCount >= 10))
                {
                    hasDecal3Texture = reader.ReadBoolean();
                }
            }
            if ((int)Version >= 0x14020007)
            {
                if ((TextureCount >= 12))
                {
                    hasDecal3Texture = reader.ReadBoolean();
                }
            }
            if (hasDecal3Texture)
            {
                Decal3Texture = new TexDesc(file, reader);
            }
            if ( (int)Version >= 0x0A000100 )
            {
                NumShaderTextures = reader.ReadUInt32();
                int num = 0;
                while ((long)num < (long)((ulong)NumShaderTextures))
                {
                    if (reader.ReadBoolean(Version))
                    {
                        new TexDesc(file, reader);
                        reader.ReadUInt32();
                    }
                    num++;
                }
            }
        }
    }
}
