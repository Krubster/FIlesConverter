using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Niflib
{
    public class MorphWeight
    {
        public NiRef<NiInterpolator> interpolator;
        /*! Weight */
        public float weight_;

        public MorphWeight(BinaryReader reader)
        {
            interpolator = new NiRef<NiInterpolator>(reader);
            weight_ = reader.ReadSingle();
        }
    }
}
