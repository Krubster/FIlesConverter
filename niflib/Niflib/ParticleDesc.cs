using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Niflib
{
    public class ParticleDesc
    {
        /*! Unknown. */
        public Vector3 translation;
        /*! Unknown. */
        public float[] unknownFloats1;
        /*! Unknown. */
        public float unknownFloat1;
        /*! Unknown. */
        public float unknownFloat2;
        /*! Unknown. */
        public float unknownFloat3;
        /*! Unknown. */
        public int unknownInt1;

        public ParticleDesc()
        {
            translation = Vector3.Zero;
            unknownFloats1 = new float[3];
            unknownFloat1 = 0.0f;
            unknownFloat2 = 0.0f;
            unknownFloat3 = 0.0f;
            unknownInt1 = 0;
        }
    }
}
