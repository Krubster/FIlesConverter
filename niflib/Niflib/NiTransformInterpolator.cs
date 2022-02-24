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
    /// Class NiTransformInterpolator.
    /// </summary>
    public class NiTransformInterpolator : NiKeyBasedInterpolator
    {
        /// <summary>
        /// The target
        /// </summary>
        public NiRef<NiObject> Target;

        public Vector3 Translation;
        public Vector4 Rotation;
        public float Scale;
        /// <summary>
        /// Initializes a new instance of the <see cref="NiTransformInterpolator" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiTransformInterpolator(NiFile file, BinaryReader reader) : base(file, reader)
        {
            Translation = reader.ReadVector3();
            Rotation = reader.ReadVector4();
            Scale = reader.ReadSingle();
            if (((int)file.Header.Version >= 0x0A01006A) && ((int)file.Header.Version <= 0x0A01006A))
            {
                for (int i2 = 0; i2 < 3; i2++)
                {
                    reader.ReadByte();
                }
            }
            Target = new NiRef<NiObject>(reader);
        }
    }
}
