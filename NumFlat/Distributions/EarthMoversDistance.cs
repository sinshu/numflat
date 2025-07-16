using System;
using EmdFlat;

namespace NumFlat.Distributions
{
    /// <summary>
    /// Provides methods for computing the Earth Mover's Distance.
    /// </summary>
    public static unsafe class EarthMoversDistance
    {
        /// <summary>
        /// Computes the Earth Mover's Distance (EMD) between two signatures using the given distance function.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the features in the signatures.
        /// </typeparam>
        /// <param name="signature1">
        /// The first signature containing features and weights.
        /// </param>
        /// <param name="signature2">
        /// The second signature containing features and weights.
        /// </param>
        /// <param name="dm">
        /// A delegate that computes the distance between two features.
        /// </param>
        /// <param name="maxIterations">
        /// The maximum number of iterations.
        /// If this limit is exceeded, an exception is thrown.
        /// </param>
        /// <returns>
        /// The Earth Mover's Distance between the two signatures.
        /// </returns>
        public static double GetDistance<T>(EmdSignature<T> signature1, EmdSignature<T> signature2, DistanceMetric<T, T> dm, int maxIterations = 500)
        {
            ThrowHelper.ThrowIfNull(signature1, nameof(signature1));
            ThrowHelper.ThrowIfNull(signature2, nameof(signature2));
            ThrowHelper.ThrowIfNull(dm, nameof(dm));

            var s1 = new signature_t<T>(signature1.Features.Count, signature1.Features, signature1.Weights);
            var s2 = new signature_t<T>(signature2.Features.Count, signature2.Features, signature2.Weights);
            var emd = new Emd(s1.n, s2.n, maxIterations);

            try
            {
                return emd.emd(s1, s2, (x, y) => dm(x, y), null, null);
            }
            catch (EmdException e)
            {
                throw new NumFlatException("Failed to compute the EMD.", e);
            }
        }

        /// <summary>
        /// Computes the Earth Mover's Distance (EMD) between two signatures using the given distance function.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the features in the signatures.
        /// </typeparam>
        /// <param name="signature1">
        /// The first signature containing features and weights.
        /// </param>
        /// <param name="signature2">
        /// The second signature containing features and weights.
        /// </param>
        /// <param name="dm">
        /// A delegate that computes the distance between two features.
        /// </param>
        /// <param name="maxIterations">
        /// The maximum number of iterations.
        /// If this limit is exceeded, an exception is thrown.
        /// </param>
        /// <returns>
        /// The Earth Mover's Distance between the two signatures and the resulting flow.
        /// </returns>
        public static (double Distance, EmdFlow[] Flow) GetDistanceAndFlow<T>(EmdSignature<T> signature1, EmdSignature<T> signature2, DistanceMetric<T, T> dm, int maxIterations = 500)
        {
            var s1 = new signature_t<T>(signature1.Features.Count, signature1.Features, signature1.Weights);
            var s2 = new signature_t<T>(signature2.Features.Count, signature2.Features, signature2.Weights);
            var emd = new Emd(s1.n, s2.n, maxIterations);

            var umFlow = new flow_t[s1.n + s2.n];
            var flowSize = 0;

            double value;
            fixed (flow_t* p = umFlow)
            {
                try
                {
                    value = emd.emd(s1, s2, (x, y) => dm(x, y), p, &flowSize);
                }
                catch (EmdException e)
                {
                    throw new NumFlatException("Failed to compute the EMD.", e);
                }
            }

            var flow = new EmdFlow[flowSize];
            for (var i = 0; i < flow.Length; i++)
            {
                var um = umFlow[i];
                flow[i] = new EmdFlow(um.from, um.to, um.amount);
            }

            return (value, flow);
        }
    }
}
