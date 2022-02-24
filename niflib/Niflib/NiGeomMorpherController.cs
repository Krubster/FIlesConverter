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
    /// Class NiGeomMorpherController.
    /// </summary>
    public class NiGeomMorpherController : NiInterpController
    {
        /// <summary>
        /// The extra flags
        /// </summary>
        public ushort ExtraFlags;

        /// <summary>
        /// The unknown2
        /// </summary>
        public byte Unknown2;

        /// <summary>
        /// The data
        /// </summary>
        public NiRef<NiMorphData> Data;

        /// <summary>
        /// The always update
        /// </summary>
        public bool AlwaysUpdate;

        /// <summary>
        /// The number interpolators
        /// </summary>
        public uint NumInterpolators;

        /// <summary>
        /// The interpolators
        /// </summary>
        public NiRef<NiInterpolator>[] Interpolators;

        /*! Weighted Interpolators? */
        MorphWeight[] interpolatorWeights;

        /// <summary>
        /// The number unkown ints
        /// </summary>
        public uint NumUnkownInts;

        /// <summary>
        /// The unkown ints
        /// </summary>
        public uint[] UnkownInts;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiGeomMorpherController" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">Version too new!</exception>
        public NiGeomMorpherController(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if (Version >= eNifVersion.VER_10_0_1_2)
            {
                ExtraFlags = reader.ReadUInt16();
            }
            if (Version == eNifVersion.VER_10_1_0_106)
            {
                Unknown2 = reader.ReadByte();
            }
            Data = new NiRef<NiMorphData>(reader);
            AlwaysUpdate = reader.ReadBoolean(Version);
            if (Version >= eNifVersion.VER_10_1_0_106)
            {
                NumInterpolators = reader.ReadUInt32();
            }
            if (Version >= eNifVersion.VER_10_1_0_106 && Version < eNifVersion.VER_20_2_0_7)
            {
                Interpolators = new NiRef<NiInterpolator>[NumInterpolators];
                int num = 0;
                while ((long)num < (long)((ulong)NumInterpolators))
                {
                    Interpolators[num] = new NiRef<NiInterpolator>(reader);
                    num++;
                }
            }
            if ((int)Version >= 0x14010003)
            {
                interpolatorWeights = new MorphWeight[NumInterpolators];
                for (int i2 = 0; i2 < interpolatorWeights.Length; i2++)
                {
                    interpolatorWeights[i2] = new MorphWeight(reader);
                };
            };
            if (((int)Version >= 0x14000004) && ((int)Version <= 0x14000005) && ((file.Header.UserVersion >= 10)))
            {
                NumUnkownInts = reader.ReadUInt32();
                UnkownInts = new uint[NumUnkownInts];
                for (int i2 = 0; i2 < UnkownInts.Length; i2++)
                {
                    UnkownInts[i2] = reader.ReadUInt32();
                }
            }
            if ((int)Version >= 0x14020007)
            {
                Interpolators = new NiRef<NiInterpolator>[NumInterpolators];
                for (int i2 = 0; i2 < NumInterpolators; i2++)
                {
                    Interpolators[i2] = interpolatorWeights[i2].interpolator;
                }
            }
            else
            {
                interpolatorWeights = new MorphWeight[NumInterpolators];
                for (int i2 = 0; i2 < NumInterpolators; i2++)
                {
                    interpolatorWeights[i2].interpolator = Interpolators[i2];
                }
            }
        }
    }
}
