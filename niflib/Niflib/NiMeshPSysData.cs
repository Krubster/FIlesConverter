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
    /// Class NiMeshPSysData.
    /// </summary>
    public class NiMeshPSysData : NiPSysData
    {
        /*! Unknown. Possible vertex count but probably not. */
        uint unknownInt2;
        /*! Unknown. 0? */
        byte unknownByte3;
        /*! Unknown. */
        uint numUnknownInts1;
        /*! Unknown integers */
        uint[] unknownInts1;
        /*! Unknown NiNode. */
        NiRef<NiNode> unknownNode;
        /// <summary>
        /// Initializes a new instance of the <see cref="NiMeshPSysData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiMeshPSysData(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if ((int)Version >= 0x0A020000)
            {
                unknownInt2 = reader.ReadUInt32();
                unknownByte3 = reader.ReadByte();
                numUnknownInts1 = reader.ReadUInt32();
                unknownInts1 = new uint[numUnknownInts1];
                for (int i2 = 0; i2 < unknownInts1.Length; i2++)
                {
                    unknownInts1[i2] = reader.ReadUInt32();
                };
                unknownNode = new NiRef<NiNode>(reader);
            };
        }
    }
}
