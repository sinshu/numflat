using System;
using NumFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides vector distance metrics.
    /// </summary>
    public static class VectorDistances
    {
        private static EuclideanDistance? euclidean = null;

        /// <summary>
        /// Gets the Euclidean distance metric.
        /// </summary>
        public static IDistance<Vec<double>, Vec<double>> Euclidean
        {
            get
            {
                if (euclidean == null)
                {
                    euclidean = new EuclideanDistance();
                }

                return euclidean;
            }
        }

        private sealed class EuclideanDistance : IDistance<Vec<double>, Vec<double>>
        {
            public double GetDistance(Vec<double> x, Vec<double> y)
            {
                return x.Distance(y);
            }
        }
    }
}
