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
	#elif SharpDX
	using SharpDX;
	#elif MonoGame
	using Microsoft.Xna.Framework;
	#endif
	using System;
	using System.IO;

    /// <summary>
    /// Class VecKey.
    /// </summary>
    public class VecKey : BaseKey
	{
        /// <summary>
        /// The time
        /// </summary>
        public float Time;

        /// <summary>
        /// The value
        /// </summary>
        public Vector3 Value;

        /// <summary>
        /// The forward
        /// </summary>
        public Vector3 Forward;

        /// <summary>
        /// The backward
        /// </summary>
        public Vector3 Backward;

        /// <summary>
        /// The TBC
        /// </summary>
        public Vector3 TBC;

        /// <summary>
        /// Initializes a new instance of the <see cref="VecKey"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="type">The type.</param>
        /// <exception cref="Exception">Invalid eKeyType!</exception>
        public VecKey(BinaryReader reader, eKeyType type) : base(reader, type)
		{
			Time = reader.ReadSingle();
			if (type < eKeyType.LINEAR_KEY || type > eKeyType.TBC_KEY)
			{
				throw new Exception("Invalid eKeyType!");
			}
			if (type == eKeyType.LINEAR_KEY)
			{
				Value = reader.ReadVector3();
			}
			if (type == eKeyType.QUADRATIC_KEY)
			{
				Value = reader.ReadVector3();
				Forward = reader.ReadVector3();
				Backward = reader.ReadVector3();
			}
			if (type == eKeyType.TBC_KEY)
			{
				Value = reader.ReadVector3();
				TBC = reader.ReadVector3();
			}
		}
	}
}
