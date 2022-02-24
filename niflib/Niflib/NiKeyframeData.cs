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
    /// Class NiKeyframeData.
    /// </summary>
    public class NiKeyframeData : NiObject
	{
        /// <summary>
        /// The key type
        /// </summary>
        public eKeyType KeyType;

        /// <summary>
        /// The quaternion keys
        /// </summary>
        public QuatKey[] QuaternionKeys;

        /// <summary>
        /// The unkown float
        /// </summary>
        public float UnkownFloat;

        /// <summary>
        /// The rotations
        /// </summary>
        public KeyGroup<FloatKey>[] Rotations;

        /// <summary>
        /// The translations
        /// </summary>
        public KeyGroup<VecKey> Translations;

        /// <summary>
        /// The scales
        /// </summary>
        public KeyGroup<FloatKey> Scales;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiKeyframeData" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiKeyframeData(NiFile file, BinaryReader reader) : base(file, reader)
		{
			uint num = reader.ReadUInt32();
			if (num != 0u)
			{
				KeyType = (eKeyType)reader.ReadUInt32();
			}
			if (KeyType != eKeyType.XYZ_ROTATION_KEY)
			{
				QuaternionKeys = new QuatKey[num];
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					QuaternionKeys[num2] = new QuatKey(reader, KeyType);
					num2++;
				}
			}
			if ((int)Version <= 0x0A010000 && KeyType == eKeyType.XYZ_ROTATION_KEY)
			{
				UnkownFloat = reader.ReadSingle();
			}
			if (KeyType == eKeyType.XYZ_ROTATION_KEY)
			{
				Rotations = new KeyGroup<FloatKey>[3];
				for (int i = 0; i < 3; i++)
				{
					Rotations[i] = new KeyGroup<FloatKey>(reader);
				}
			}
			Translations = new KeyGroup<VecKey>(reader);
			Scales = new KeyGroup<FloatKey>(reader);
		}
	}
}
