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
    /// Class NiPSysEmitter.
    /// </summary>
    public class NiPSysEmitter : NiPSysModifier
    {
        /*! Speed / Inertia of particle movement. */
        public float speed;
        /*! Adds an amount of randomness to Speed. */
        public float speedVariation;
        /*! Declination / First axis. */
        public float declination;
        /*! Declination randomness / First axis. */
        public float declinationVariation;
        /*! Planar Angle / Second axis. */
        public float planarAngle;
        /*! Planar Angle randomness / Second axis . */
        public float planarAngleVariation;
        /*! Defines color of a birthed particle. */
        public Vector4 initialColor;
        /*! Size of a birthed particle. */
        public float initialRadius;
        /*! Particle Radius randomness. */
        public float radiusVariation;
        /*! Duration until a particle dies. */
        public float lifeSpan;
        /*! Adds randomness to Life Span. */
        public float lifeSpanVariation;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysEmitter"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysEmitter(NiFile file, BinaryReader reader) : base(file, reader)
        {
            speed = reader.ReadSingle();
            speedVariation = reader.ReadSingle();
            declination = reader.ReadSingle();
            declinationVariation = reader.ReadSingle();
            planarAngle = reader.ReadSingle();
            planarAngleVariation = reader.ReadSingle();
            initialColor = reader.ReadVector4();
            initialRadius = reader.ReadSingle();
            if ((int)file.Version >= 0x0A040001)
            {
                radiusVariation = reader.ReadSingle();
            };
            lifeSpan = reader.ReadSingle();
            lifeSpanVariation = reader.ReadSingle();
        }
    }
}
