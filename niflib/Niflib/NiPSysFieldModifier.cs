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
    /// Class NiPSysFieldModifier.
    /// </summary>
    public class NiPSysFieldModifier : NiPSysModifierFloatCtlr
    {
        /*! Force Field Object */
        NiRef<NiAVObject> fieldObject;
        /*! Magnitude of the force */
        float magnitude;
        /*! Controls how quick the field diminishes */
        float attenuation;
        /*! Use maximum distance */
        bool useMaxDistance;
        /*! Maximum distance */
        float maxDistance;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysFieldModifier"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysFieldModifier(NiFile file, BinaryReader reader) : base(file, reader)
        {
            fieldObject = new NiRef<NiAVObject>(reader);
            magnitude = reader.ReadSingle();
            attenuation = reader.ReadSingle();
            useMaxDistance = reader.ReadBoolean(Version);
            maxDistance = reader.ReadSingle();
        }
    }
}
