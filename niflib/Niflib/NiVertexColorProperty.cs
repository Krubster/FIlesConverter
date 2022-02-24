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
    /// Class NiVertexColorProperty.
    /// </summary>
    public class NiVertexColorProperty : NiProperty
    {
        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The vertex mode
        /// </summary>
        public uint VertexMode;

        /// <summary>
        /// The lighting mode
        /// </summary>
        public uint LightingMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiVertexColorProperty"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">unsupported data!</exception>
        public NiVertexColorProperty(NiFile file, BinaryReader reader) : base(file, reader)
        {
            Flags = reader.ReadUInt16();
            if ((int)Version <= 0x14000005)
            {
                VertexMode = reader.ReadUInt32();
                LightingMode = reader.ReadUInt32();
            }
            if ((int)Version > 0x14000005)
            {
                LightingMode = (uint)UnpackField(Flags, 3, 1);
                VertexMode = (uint)UnpackField(Flags, 4, 2);
            }
        }

        public int UnpackField(ushort flags, int lshift, int num_bits)
        {
            int mask = 0;
            for (int i = lshift; i < num_bits + lshift; ++i)
            {
                mask |= 1 << i;
            }
            return (int)((flags & mask) >> lshift);
        }
    }
}
