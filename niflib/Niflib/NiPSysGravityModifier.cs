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

    enum ForceType
    {
        FORCE_PLANAR = 0, /*!< FORCE_PLANAR */
        FORCE_SPHERICAL = 1, /*!< FORCE_SPHERICAL */
        FORCE_UNKNOWN = 2, /*!< FORCE_UNKNOWN */
    };

    /// <summary>
    /// Class NiPSysGravityModifier.
    /// </summary>
    public class NiPSysGravityModifier : NiPSysModifier
    {
        /*! Refers to a NiNode for gravity location. */
        NiRef<NiNode> gravityObject;
        /*! Orientation of gravity. */
        Vector3 gravityAxis;
        /*! Falloff range. */
        float decay;
        /*! The strength of gravity. */
        float strength;
        /*! Planar or Spherical type */
        ForceType forceType;
        /*! Adds a degree of randomness. */
        float turbulence;
        /*! Range for turbulence. */
        float turbulenceScale;
        /*! Unknown */
        byte unknownByte;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysGravityModifier"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysGravityModifier(NiFile file, BinaryReader reader) : base(file, reader)
        {
            gravityObject = new NiRef<NiNode>(reader);
            gravityAxis = reader.ReadVector3();
            decay = reader.ReadSingle();
            strength = reader.ReadSingle();
            forceType = (ForceType)reader.ReadUInt32();
            turbulence = reader.ReadSingle();
            turbulenceScale = reader.ReadSingle();
            if (((int)file.Version >= 0x14020007) && ((file.Header.UserVersion >= 11)))
            {
                unknownByte = reader.ReadByte();
            };
        }
    }
}
