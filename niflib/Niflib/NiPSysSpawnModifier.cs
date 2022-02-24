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
    /// Class NiPSysSpawnModifier.
    /// </summary>
    public class NiPSysSpawnModifier : NiPSysModifier
    {
        public ushort numSpawnGenerations;
        /*! Unknown. */
        public float percentageSpawned;
        /*! Unknown. */
        public ushort minNumToSpawn;
        /*! Unknown. */
        public ushort maxNumToSpawn;
        /*! Unknown. */
        public float spawnSpeedChaos;
        /*! Unknown. */
        public float spawnDirChaos;
        /*! Unknown. */
        public float lifeSpan;
        /*! Unknown. */
        public float lifeSpanVariation;
        /*! Unknown */
        public int unknownInt;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysSpawnModifier"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysSpawnModifier(NiFile file, BinaryReader reader) : base(file, reader)
        {
            numSpawnGenerations = reader.ReadUInt16();
            percentageSpawned = reader.ReadSingle();
            minNumToSpawn = reader.ReadUInt16();
            maxNumToSpawn = reader.ReadUInt16();
            spawnSpeedChaos = reader.ReadSingle();
            spawnDirChaos = reader.ReadSingle();
            lifeSpan = reader.ReadSingle();
            lifeSpanVariation = reader.ReadSingle();
            if (((int)file.Version >= 0x0A040001) && ((int)file.Version <= 0x0A040001))
            {
                unknownInt = reader.ReadInt32();
            }
        }
    }
}
