using System;
using NumFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides vector distances.
    /// </summary>
    public static class VectorDistances
    {
        private static EuclideanDistance? euclidean = null;

        /// <summary>
        /// Gets the Euclidean distance measure.
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
