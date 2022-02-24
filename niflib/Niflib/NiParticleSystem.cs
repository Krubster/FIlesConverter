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
    /// Class NiParticleSystem.
    /// </summary>
    public class NiParticleSystem : NiGeometry
    {
        public bool WorldSpace;
        public NiRef<NiPSysModifier>[] Modifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticleSystem"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticleSystem(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if ((file.Header.UserVersion >= 12))
            {
                reader.ReadUInt16();
                reader.ReadUInt16();
                reader.ReadUInt32();
            }
            if ( (int)file.Header.Version >= 0x0A010000)
            {
                WorldSpace = reader.ReadBoolean();
                uint numModifiers = reader.ReadUInt32();
                Modifiers = new NiRef<NiPSysModifier>[numModifiers];
                for (int i2 = 0; i2 < Modifiers.Length; i2++)
                {
                    Modifiers[i2] = new NiRef<NiPSysModifier>(reader); 
                };
            };
        }
    }
}
