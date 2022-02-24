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
    /// Class NiPathInterpolator.
    /// </summary>
    public class NiPathInterpolator : NiKeyBasedInterpolator
    {
        /*! Unknown. */
        ushort unknownShort;
        /*! Unknown. */
        uint unknownInt;
        /*! Unknown. */
        float unknownFloat1;
        /*! Unknown. */
        float unknownFloat2;
        /*! Unknown. Zero. */
        ushort unknownShort2;
        /*! Links to NiPosData. */
        NiRef<NiPosData> posData;
        /*! Links to NiFloatData. */
        NiRef<NiFloatData> floatData;
        /// <summary>
        /// Initializes a new instance of the <see cref="NiPathInterpolator" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPathInterpolator(NiFile file, BinaryReader reader) : base(file, reader)
        {
            unknownShort = reader.ReadUInt16();
            unknownInt = reader.ReadUInt32();
            unknownFloat1 = reader.ReadSingle();
            unknownFloat2 = reader.ReadSingle();
            unknownShort2 = reader.ReadUInt16();
            posData = new NiRef<NiPosData>(reader);
            floatData = new NiRef<NiFloatData>(reader);
        }
    }
}
