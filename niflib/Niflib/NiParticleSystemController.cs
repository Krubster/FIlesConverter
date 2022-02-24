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
    /// Class NiParticleSystemController.
    /// </summary>
    public class NiParticleSystemController : NiTimeController
	{
        /// <summary>
        /// The old speed
        /// </summary>
        public uint OldSpeed;

        /// <summary>
        /// The speed
        /// </summary>
        public float Speed;

        /// <summary>
        /// The random speed
        /// </summary>
        public float RandomSpeed;

        /// <summary>
        /// The vertical direction
        /// </summary>
        public float VerticalDirection;

        /// <summary>
        /// The vertical angle
        /// </summary>
        public float VerticalAngle;

        /// <summary>
        /// The horizontal direction
        /// </summary>
        public float HorizontalDirection;

        /// <summary>
        /// The horizontal angle
        /// </summary>
        public float HorizontalAngle;

        /// <summary>
        /// The unkown normal
        /// </summary>
        public Vector3 UnkownNormal;

        /// <summary>
        /// The unkown color
        /// </summary>
        public Color4 UnkownColor;

        /// <summary>
        /// The size
        /// </summary>
        public float Size;

        /// <summary>
        /// The emit start time
        /// </summary>
        public float EmitStartTime;

        /// <summary>
        /// The emit stop time
        /// </summary>
        public float EmitStopTime;

        /// <summary>
        /// The unkown byte
        /// </summary>
        public byte UnkownByte;

        /// <summary>
        /// The old emit rate
        /// </summary>
        public uint OldEmitRate;

        /// <summary>
        /// The emit rate
        /// </summary>
        public float EmitRate;

        /// <summary>
        /// The lifetime
        /// </summary>
        public float Lifetime;

        /// <summary>
        /// The lifetime random
        /// </summary>
        public float LifetimeRandom;

        /// <summary>
        /// The emit flags
        /// </summary>
        public ushort EmitFlags;

        /// <summary>
        /// The start random
        /// </summary>
        public Vector3 StartRandom;

        /// <summary>
        /// The emitter
        /// </summary>
        public NiRef<NiObject> Emitter;

        /// <summary>
        /// The particle velocity
        /// </summary>
        public Vector3 ParticleVelocity;

        /// <summary>
        /// The particle unkown vector
        /// </summary>
        public Vector3 ParticleUnkownVector;

        /// <summary>
        /// The particle life time
        /// </summary>
        public float ParticleLifeTime;

        /// <summary>
        /// The particle link
        /// </summary>
        public NiRef<NiObject> ParticleLink;

        /// <summary>
        /// The particle timestamp
        /// </summary>
        public uint ParticleTimestamp;

        /// <summary>
        /// The particle unkown short
        /// </summary>
        public ushort ParticleUnkownShort;

        /// <summary>
        /// The particle vertex identifier
        /// </summary>
        public ushort ParticleVertexId;

        /// <summary>
        /// The number particles
        /// </summary>
        public ushort NumParticles;

        /// <summary>
        /// The number valid
        /// </summary>
        public ushort NumValid;

        /// <summary>
        /// The particles
        /// </summary>
        public Particle[] Particles;

        /// <summary>
        /// The unkown reference
        /// </summary>
        public NiRef<NiObject> UnkownRef;

        /// <summary>
        /// The particle extra
        /// </summary>
        public NiRef<NiParticleModifier> ParticleExtra;

        /// <summary>
        /// The unkown ref2
        /// </summary>
        public NiRef<NiObject> UnkownRef2;

        /// <summary>
        /// The trailer
        /// </summary>
        public byte Trailer;

        /// <summary>
        /// The color data
        /// </summary>
        public NiRef<NiColorData> ColorData;

        /// <summary>
        /// The unkown float1
        /// </summary>
        public float UnkownFloat1;

        /// <summary>
        /// The unkown floats2
        /// </summary>
        public float[] UnkownFloats2;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiParticleSystemController" /> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiParticleSystemController(NiFile file, BinaryReader reader) : base(file, reader)
		{
			if (Version <= eNifVersion.VER_3_1)
			{
				OldSpeed = reader.ReadUInt32();
			}
			if (Version >= eNifVersion.VER_3_3_0_13)
			{
				Speed = reader.ReadSingle();
			}
			RandomSpeed = reader.ReadSingle();
			VerticalDirection = reader.ReadSingle();
			VerticalAngle = reader.ReadSingle();
			HorizontalDirection = reader.ReadSingle();
			HorizontalAngle = reader.ReadSingle();
			UnkownNormal = reader.ReadVector3();
			UnkownColor = reader.ReadColor4();
			Size = reader.ReadSingle();
			EmitStartTime = reader.ReadSingle();
			EmitStopTime = reader.ReadSingle();
			if (Version >= eNifVersion.VER_4_0_0_2)
			{
				UnkownByte = reader.ReadByte();
			}
			if (Version <= eNifVersion.VER_3_1)
			{
				OldEmitRate = reader.ReadUInt32();
			}
			if (Version >= eNifVersion.VER_3_3_0_13)
			{
				EmitRate = reader.ReadSingle();
			}
			Lifetime = reader.ReadSingle();
			LifetimeRandom = reader.ReadSingle();
			if (Version >= eNifVersion.VER_4_0_0_2)
			{
				EmitFlags = reader.ReadUInt16();
			}
			StartRandom = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			Emitter = new NiRef<NiObject>(reader);
			if (Version >= eNifVersion.VER_4_0_0_2)
			{
				reader.ReadUInt16();
				reader.ReadSingle();
				reader.ReadUInt32();
				reader.ReadUInt32();
				reader.ReadUInt16();
			}
			if (Version <= eNifVersion.VER_3_1)
			{
				ParticleVelocity = reader.ReadVector3();
				ParticleUnkownVector = reader.ReadVector3();
				ParticleLifeTime = reader.ReadSingle();
				ParticleLink = new NiRef<NiObject>(reader);
				ParticleTimestamp = reader.ReadUInt32();
				ParticleUnkownShort = reader.ReadUInt16();
				ParticleVertexId = reader.ReadUInt16();
			}
			if (Version >= eNifVersion.VER_4_0_0_2)
			{
				NumParticles = reader.ReadUInt16();
				NumValid = reader.ReadUInt16();
				Particles = new Particle[(int)NumParticles];
				for (int i = 0; i < (int)NumParticles; i++)
				{
					Particles[i] = new Particle(file, reader);
				}
				UnkownRef = new NiRef<NiObject>(reader);
			}
			ParticleExtra = new NiRef<NiParticleModifier>(reader);
			UnkownRef2 = new NiRef<NiObject>(reader);
			if (Version >= eNifVersion.VER_4_0_0_2)
			{
				Trailer = reader.ReadByte();
			}
			if (Version <= eNifVersion.VER_3_1)
			{
				ColorData = new NiRef<NiColorData>(reader);
				UnkownFloat1 = reader.ReadSingle();
				UnkownFloats2 = reader.ReadFloatArray((int)ParticleUnkownShort);
			}
		}
	}
}
