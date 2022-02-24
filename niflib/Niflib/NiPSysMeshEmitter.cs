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

    enum VelocityType
    {
        VELOCITY_USE_NORMALS = 0, /*!< Uses the normals of the meshes to determine staring velocity. */
        VELOCITY_USE_RANDOM = 1, /*!< Starts particles with a random velocity. */
        VELOCITY_USE_DIRECTION = 2, /*!< Uses the emission axis to determine initial particle direction? */
    }

    enum EmitFrom
    {
        EMIT_FROM_VERTICES = 0, /*!< Emit particles from the vertices of the mesh. */
        EMIT_FROM_FACE_CENTER = 1, /*!< Emit particles from the center of the faces of the mesh. */
        EMIT_FROM_EDGE_CENTER = 2, /*!< Emit particles from the center of the edges of the mesh. */
        EMIT_FROM_FACE_SURFACE = 3, /*!< Perhaps randomly emit particles from anywhere on the faces of the mesh? */
        EMIT_FROM_EDGE_SURFACE = 4, /*!< Perhaps randomly emit particles from anywhere on the edges of the mesh? */
    }

    /// <summary>
    /// Class NiPSysMeshEmitter.
    /// </summary>
    public class NiPSysMeshEmitter : NiPSysEmitter
    {
        /*! The number of references to emitter meshes that follow. */
        uint numEmitterMeshes;
        /*! Links to meshes used for emitting. */
        NiRef<NiTriBasedGeometry>[] emitterMeshes;
        /*! The way the particles get their initial direction and speed. */
        VelocityType initialVelocityType;
        /*! The parts of the mesh that the particles emit from. */
        EmitFrom emissionType;
        /*! The emission axis. */
        Vector3 emissionAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysMeshEmitter"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysMeshEmitter(NiFile file, BinaryReader reader) : base(file, reader)
        {
            numEmitterMeshes = reader.ReadUInt32();
            emitterMeshes = new NiRef<NiTriBasedGeometry>[numEmitterMeshes];
            for (uint i1 = 0; i1 < emitterMeshes.Length; i1++)
            {
                emitterMeshes[i1] = new NiRef<NiTriBasedGeometry>(reader);
            };
            initialVelocityType = (VelocityType)reader.ReadUInt32();
            emissionType = (EmitFrom)reader.ReadUInt32();
            emissionAxis = reader.ReadVector3();
        }
    }
}
