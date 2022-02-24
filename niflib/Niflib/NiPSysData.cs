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
    using OpenTK;
    using System;
    using System.IO;

    /// <summary>
    /// Class NiPSysData.
    /// </summary>
    public class NiPSysData : NiRotatingParticlesData
    {

        /*! Unknown. */
        public ParticleDesc[] particleDescriptions;
        /*! Unknown. */
        public bool hasUnknownFloats3;
        /*! Unknown. */
        public float[] unknownFloats3;
        /*! Unknown. */
        public ushort unknownShort1;
        /*! Unknown. */
        public ushort unknownShort2;
        /*! Boolean for Num Subtexture Offset UVs */
        public bool hasSubtextureOffsetUvs;
        /*! How many quads to use in BSPSysSubTexModifier for texture atlasing */
        public uint numSubtextureOffsetUvs;
        /*! Sets aspect ratio for Subtexture Offset UV quads */
        public float aspectRatio;
        /*! Defines UV offsets */
        public Vector4[] subtextureOffsetUvs;
        /*! Unknown */
        public uint unknownInt4;
        /*! Unknown */
        public uint unknownInt5;
        /*! Unknown */
        public uint unknownInt6;
        /*! Unknown */
        public ushort unknownShort3;
        /*! Unknown */
        public byte unknownByte4;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysData(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if ((!(((int)file.Version >= 0x14020007) && (file.Header.UserVersion >= 11))))
            {
                particleDescriptions = new ParticleDesc[NumVertices];
                for (uint i2 = 0; i2 < particleDescriptions.Length; i2++)
                {
                    particleDescriptions[i2] = new ParticleDesc();
                    particleDescriptions[i2].translation = reader.ReadVector3();
                    if ((int)file.Version <= 0x0A040001)
                    {
                        for (uint i4 = 0; i4 < 3; i4++)
                        {
                            particleDescriptions[i2].unknownFloats1[i4] = reader.ReadSingle();
                        };
                    };
                    particleDescriptions[i2].unknownFloat1 = reader.ReadSingle();
                    particleDescriptions[i2].unknownFloat2 = reader.ReadSingle();
                    particleDescriptions[i2].unknownFloat3 = reader.ReadSingle();
                    particleDescriptions[i2].unknownInt1 = reader.ReadInt32();
                };
            };
            if (((int)file.Version >= 0x14000004) && ((!(((int)file.Version >= 0x14020007) && (file.Header.UserVersion >= 11)))))
            {
                hasUnknownFloats3 = reader.ReadBoolean();
                if (hasUnknownFloats3)
                {
                    unknownFloats3 = new float[NumVertices];
                    for (uint i3 = 0; i3 < unknownFloats3.Length; i3++)
                    {
                        unknownFloats3[i3] = reader.ReadSingle();
                    };
                };
            };
            if ((!(((int)file.Version >= 0x14020007) && ((int)file.Header.UserVersion == 11))))
            {
                unknownShort1 = reader.ReadUInt16();
                unknownShort2 = reader.ReadUInt16();
            };
            if ((((int)file.Version >= 0x14020007) && ((int)file.Header.UserVersion >= 12)))
            {
                hasSubtextureOffsetUvs = reader.ReadBoolean();
                numSubtextureOffsetUvs = reader.ReadUInt32();
                aspectRatio = reader.ReadSingle();
                if (hasSubtextureOffsetUvs)
                {
                    subtextureOffsetUvs = new Vector4[numSubtextureOffsetUvs];
                    for (uint i3 = 0; i3 < subtextureOffsetUvs.Length; i3++)
                    {
                        subtextureOffsetUvs[i3] = reader.ReadVector4();
                    };
                };
                unknownInt4 = reader.ReadUInt32();
                unknownInt5 = reader.ReadUInt32();
                unknownInt6 = reader.ReadUInt32();
                unknownShort3 = reader.ReadUInt16();
                unknownByte4 = reader.ReadByte();
            }
        }
    }
}