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
    using Matrix = OpenTK.Matrix4;
#elif SharpDX
	using SharpDX;
#elif MonoGame
	using Microsoft.Xna.Framework;
#endif
    using System;
    using System.IO;

    /// <summary>
    /// Class NiAVObject.
    /// </summary>
    public class NiAVObject : NiObjectNET
    {
        /// <summary>
        /// The flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// The unkown short1
        /// </summary>
        public ushort UnkownShort1;

        /// <summary>
        /// The translation
        /// </summary>
        public Vector3 Translation;

        /// <summary>
        /// The rotation
        /// </summary>
        public Matrix Rotation;

        /// <summary>
        /// The scale
        /// </summary>
        public float Scale;

        /// <summary>
        /// The velocity
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// The properties
        /// </summary>
        public NiRef<NiProperty>[] Properties;

        /// <summary>
        /// The unkown ints1
        /// </summary>
        public uint[] UnkownInts1;

        /// <summary>
        /// The unkown byte
        /// </summary>
        public byte UnkownByte;

        /// <summary>
        /// The has bounding box
        /// </summary>
        public bool HasBoundingBox;

        /// <summary>
        /// The collision object
        /// </summary>
        public NiRef<NiCollisionObject> CollisionObject;

        /// <summary>
        /// The parent
        /// </summary>
        public NiNode Parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiAVObject" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        /// <exception cref="Exception">Cannot read BoundingBoxes yet</exception>
        public NiAVObject(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if (File.Header.Version >= eNifVersion.VER_3_0)
            {
                Flags = reader.ReadUInt16();
            }
            if (File.Header.Version >= eNifVersion.VER_20_2_0_7 && File.Header.UserVersion == 11u && File.Header.UserVersion2 > 26u)
            {
                UnkownShort1 = reader.ReadUInt16();
            }
            Translation = reader.ReadVector3();
            Rotation = reader.ReadMatrix33();
            Scale = reader.ReadSingle();
            if (File.Header.Version <= eNifVersion.VER_4_2_2_0)
            {
                Velocity = reader.ReadVector3();
            }
            if (File.Header.Version <= eNifVersion.VER_20_2_0_7 || File.Header.UserVersion <= 11u)
            {
                uint num = reader.ReadUInt32();
                Properties = new NiRef<NiProperty>[num];
                int num2 = 0;
                while ((long)num2 < (long)((ulong)num))
                {
                    Properties[num2] = new NiRef<NiProperty>(reader.ReadUInt32());
                    num2++;
                }
            }
            if (File.Header.Version <= eNifVersion.VER_2_3)
            {
                UnkownInts1 = new uint[4];
                for (int i = 0; i < 4; i++)
                {
                    UnkownInts1[i] = reader.ReadUInt32();
                }
                UnkownByte = reader.ReadByte();
            }
            if (File.Header.Version >= eNifVersion.VER_3_0 && File.Header.Version <= eNifVersion.VER_4_2_2_0)
            {
                HasBoundingBox = reader.ReadBoolean(Version);
                if (HasBoundingBox)
                {
                    reader.ReadUInt32(); // boundingBox.unknownInt
                    reader.ReadVector3(); // boundingBox.translation
                    for (int c = 0; c < 3; ++c) // boundingBox.rotation
                    {
                        for (int r = 0; r < 3; ++r)
                        {
                            reader.ReadSingle();
                        }
                    }
                    reader.ReadVector3(); // boundingBox.radius
                }
            }
            if ( (int)File.Header.Version >= 0x0A000100 )
            {
                uint refId = reader.ReadUInt32();
                CollisionObject = new NiRef<NiCollisionObject>(refId);
            }
        }
    }
}
