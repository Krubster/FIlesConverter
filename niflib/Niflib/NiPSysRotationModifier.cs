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
    /// Class NiPSysRotationModifier.
    /// </summary>
    public class NiPSysRotationModifier : NiPSysModifier
    {
        /*! The initial speed of rotation. */
        float initialRotationSpeed;
        /*! Adds a ranged randomness to rotation speed. */
        float initialRotationSpeedVariation;
        /*! Sets the intial angle for particles to be birthed in. */
        float initialRotationAngle;
        /*! Adds a random range to Initial angle. */
        float initialRotationAngleVariation;
        /*! Unknown */
        bool randomRotSpeedSign;
        /*! Unknown. */
        bool randomInitialAxis;
        /*! Unknown. */
        Vector3 initialAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysRotationModifier"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysRotationModifier(NiFile file, BinaryReader reader) : base(file, reader)
        {
            initialRotationSpeed = reader.ReadInt32();
            if ((int)file.Version >= 0x14000004)
            {
                initialRotationSpeedVariation = reader.ReadSingle();
                initialRotationAngle = reader.ReadSingle();
                initialRotationAngleVariation = reader.ReadSingle();
                randomRotSpeedSign = reader.ReadBoolean();
            };
            randomInitialAxis = reader.ReadBoolean();
            initialAxis = reader.ReadVector3();
        }
    }
}
