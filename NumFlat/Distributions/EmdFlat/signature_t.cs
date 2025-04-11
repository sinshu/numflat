#pragma warning disable CS1591

using System;
using System.Collections.Generic;

namespace EmdFlat
{
    public sealed class signature_t<feature_t>
    {
        public int n;                              /* Number of features in the signature */
        public IReadOnlyList<feature_t> Features;  /* Pointer to the features vector */
        public IReadOnlyList<double> Weights;      /* Pointer to the weights of the features */

        public signature_t(int n, IReadOnlyList<feature_t> Features, IReadOnlyList<double> Weights)
        {
            this.n = n;
            this.Features = Features;
            this.Weights = Weights;
        }
    }
}
