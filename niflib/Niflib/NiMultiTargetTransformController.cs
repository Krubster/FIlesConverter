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
#if OpenTK
    using OpenTK;
    using OpenTK.Graphics;
#elif SharpDX
	using SharpDX;
#elif MonoGame
	using Microsoft.Xna.Framework;
	using Color4 = Microsoft.Xna.Framework.Color;
#endif
    using System;
    using System.IO;

    /// <summary>
    /// Class NiMultiTargetTransformController.
    /// </summary>
    public class NiMultiTargetTransformController : NiInterpController
    {
        /*! The number of target pointers that follow. */
        ushort numExtraTargets;
        /*! NiNode Targets to be controlled. */
        NiRef<NiAVObject>[] extraTargets;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiMultiTargetTransformController" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiMultiTargetTransformController(NiFile file, BinaryReader reader) : base(file, reader)
        {
            numExtraTargets = reader.ReadUInt16();
            extraTargets = new NiRef<NiAVObject>[numExtraTargets];
            for (int i = 0; i < numExtraTargets; ++i)
            {
                extraTargets[i] = new NiRef<NiAVObject>(reader);
            }
        }
    }
}
