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
    /// Class NiParticlesData.
    /// </summary>
    public class NiParticlesData : NiGeometryData
    {
        /// <summary>
        /// The number particles
        /// </summary>
        public ushort NumParticles;

        /// <summary>
        /// The particle radius
        /// </summary>
        public float ParticleRadius;

        /// <summary>
        /// The has radii
        /// </summary>
        public bool HasRadii;

        /// <summary>
        /// The radii
        /// </summary>
        public float[] Radii;

        /// <summary>
        /// The number active
        /// </summary>
        public ushort NumActive;

        /// <summary>
        /// The has sizes
        /// </summary>
        public bool HasSizes;

        /// <summary>
        /// The sizes
        /// </summary>
        public float[] Sizes;

        /// <summary>
        /// The has rotations
        /// </summary>
        public bool HasRotations;

        /// <summary>
        /// The rotations
        /// </summary>
        public Vector4[] Rotations;

        /*! Unknown, probably a boolean. */
        byte unknownByte1;
        /*! Unknown */
        NiRef<NiObject> unknownLink;
        /*! Are the angles of rotation present? */
        bool hasRotationAngles;
        /*! Angles of rotation */
        float[] rotationAngles;
        /*! Are axes of rotation present? */
        bool hasRotationAxes;
        /*! Unknown */
        Vector3[] rotationAxes;
        /*! if value is no, a single image rendered */
        bool hasUvQuadrants;
        /*!
         * 2,4,8,16,32,64 are potential values. If "Has" was no then this should be 256,
         * which represents a 16x16 framed image, which is invalid
         */
        byte numUvQuadrants;
        /*! Unknown. */
        Vector4[] uvQuadrants;
        /*! Unknown */
        byte unknownByte2;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticlesData"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticlesData(NiFile file, BinaryReader reader) : base(file, reader)
        {
            if (File.Header.Version <= eNifVersion.VER_4_0_0_2)
            {
                NumParticles = reader.ReadUInt16();
            }
            if (File.Header.Version <= eNifVersion.VER_10_0_1_0)
            {
                ParticleRadius = reader.ReadSingle();
            }
            if (File.Header.Version >= eNifVersion.VER_10_1_0_0)
            {
                HasRadii = reader.ReadBoolean(Version);
                if (HasRadii)
                {
                    Radii = reader.ReadFloatArray((int)NumVertices);
                }
            }
            NumActive = reader.ReadUInt16();
            HasSizes = reader.ReadBoolean(Version);
            if (HasSizes)
            {
                Sizes = reader.ReadFloatArray((int)NumVertices);
            }
            if (File.Header.Version >= eNifVersion.VER_10_0_1_0)
            {
                HasRotations = reader.ReadBoolean(Version);
                if (HasRotations)
                {
                    Rotations = new Vector4[NumVertices];
                    int num = 0;
                    while ((long)num < (long)((ulong)NumVertices))
                    {
                        Rotations[num] = reader.ReadVector4();
                        num++;
                    }
                }
            }
            if ((((int)Version >= 0x14020007) && (file.Header.UserVersion >= 12)))
            {
                unknownByte1 = reader.ReadByte();
                unknownLink = new NiRef<NiObject>(reader);
            };
            if ((int)Version >= 0x14000004)
            {
                hasRotationAngles = reader.ReadBoolean(Version);
            };
            if ((!(((int)Version >= 0x14020007) && (file.Header.UserVersion >= 11))))
            {
                if (hasRotationAngles)
                {
                    rotationAngles = new float[NumVertices];
                    for (int i3 = 0; i3 < rotationAngles.Length; i3++)
                    {
                        rotationAngles[i3] = reader.ReadSingle();
                    };
                };
            };
            if ((int)Version >= 0x14000004)
            {
                hasRotationAxes = reader.ReadBoolean();
            };
            if (((int)Version >= 0x14000004) && ((!(((int)Version >= 0x14020007) && (file.Header.UserVersion >= 11)))))
            {
                if (hasRotationAxes)
                {
                    rotationAxes = new Vector3[NumVertices];
                    for (int i3 = 0; i3 < rotationAxes.Length; i3++)
                    {
                        rotationAxes[i3] = reader.ReadVector3();
                    };
                };
            };
            if ((((int)Version >= 0x14020007) && (file.Header.UserVersion == 11)))
            {
                hasUvQuadrants = reader.ReadBoolean();
                numUvQuadrants = reader.ReadByte();
                if (hasUvQuadrants)
                {
                    uvQuadrants = new Vector4[numUvQuadrants];
                    for (int i3 = 0; i3 < uvQuadrants.Length; i3++)
                    {
                        uvQuadrants[i3] = reader.ReadVector4();
                    };
                };
            };
            if ((((int)Version == 0x14020007) && (file.Header.UserVersion >= 11)))
            {
                unknownByte2 = reader.ReadByte();
            };
        }
    }
}
