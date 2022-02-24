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
    /// Class NiCollisionData.
    /// </summary>
    public class NiCollisionData : NiCollisionObject
    {
        public uint PropagnationMode;
        public uint CollisionMode;
        /// <summary>
        /// Initializes a new instance of the <see cref="NiCollisionData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiCollisionData(NiFile file, BinaryReader reader) : base(file, reader)
        {
            PropagnationMode = reader.ReadUInt32();
            if ((int)Version >= 0x0A010000)
            {
                CollisionMode = reader.ReadUInt32();
            }
            byte useAbv = reader.ReadByte();
            if ((useAbv == 1))
            {
                uint boundingVolume_collisionType = reader.ReadUInt32();
                if ((boundingVolume_collisionType == 0))
                {
                    Vector3 boundingVolume_sphere_center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    float boundingVolume_sphere_radius = reader.ReadSingle();
                }
                if ((boundingVolume_collisionType == 1))
                {
                    Vector3 boundingVolume_box_center = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    for (uint i3 = 0; i3 < 3; i3++)
                    {
                        new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    }
                    for (uint i3 = 0; i3 < 3; i3++)
                    {
                        reader.ReadSingle();
                    }
                }
                if ((boundingVolume_collisionType == 2))
                {
                    new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    reader.ReadSingle();
                    reader.ReadSingle();
                }
                if ((boundingVolume_collisionType == 5))
                {
                    new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                }
            }
        }
    }
}