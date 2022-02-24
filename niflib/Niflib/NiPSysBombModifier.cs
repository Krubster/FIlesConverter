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

    /*! Determines decay function.  Used by NiPSysBombModifier. */
    enum DecayType
    {
        DECAY_NONE = 0, /*!< No decay. */
        DECAY_LINEAR = 1, /*!< Linear decay. */
        DECAY_EXPONENTIAL = 2, /*!< Exponential decay. */
    }

    /*! Determines symetry type used by NiPSysBombModifier. */
    enum SymmetryType
    {
        SPHERICAL_SYMMETRY = 0, /*!< Spherical Symmetry. */
        CYLINDRICAL_SYMMETRY = 1, /*!< Cylindrical Symmetry. */
        PLANAR_SYMMETRY = 2, /*!< Planar Symmetry. */
    }

    /// <summary>
    /// Class NiPSysBombModifier.
    /// </summary>
    public class NiPSysBombModifier : NiPSysModifier
    {
        /*! Link to a NiNode for bomb to function. */
        NiRef<NiNode> bombObject;
        /*! Orientation of bomb object. */
        Vector3 bombAxis;
        /*! Falloff rate of the bomb object. */
        float decay;
        /*! DeltaV /  Strength? */
        float deltaV;
        /*! Decay type */
        DecayType decayType;
        /*! Shape/symmetry of the bomb object. */
        SymmetryType symmetryType;

        /// <summary>
        /// Initializes a new instance of the <see cref="NiPSysBombModifier"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="reader">The reader.</param>
        public NiPSysBombModifier(NiFile file, BinaryReader reader) : base(file, reader)
        {
            bombObject = new NiRef<NiNode>(reader);
            bombAxis = reader.ReadVector3();
            decay = reader.ReadSingle();
            deltaV = reader.ReadSingle();
            decayType = (DecayType)reader.ReadUInt32();
            symmetryType = (SymmetryType)reader.ReadUInt32();
        }
    }
}
