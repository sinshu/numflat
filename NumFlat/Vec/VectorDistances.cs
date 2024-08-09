using System;
using NumFlat;

namespace NumFlat
{
    /// <summary>
    /// Provides vector distance metrics.
    /// </summary>
    public static class VectorDistances
    {
        ////////////////////////////////////////////////////////////
        // Euclidean
        ////////////////////////////////////////////////////////////
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



        ////////////////////////////////////////////////////////////
        // Manhattan
        ////////////////////////////////////////////////////////////
        private static ManhattanDistance? manhattan = null;

        /// <summary>
        /// Gets the Manhattan distance metric.
        /// </summary>
        public static IDistance<Vec<double>, Vec<double>> Manhattan
        {
            get
            {
                if (manhattan == null)
                {
                    manhattan = new ManhattanDistance();
                }

                return manhattan;
            }
        }

        private sealed class ManhattanDistance : IDistance<Vec<double>, Vec<double>>
        {
            public double GetDistance(Vec<double> x, Vec<double> y)
            {
                ThrowHelper.ThrowIfEmpty(x, nameof(x));
                ThrowHelper.ThrowIfEmpty(y, nameof(y));
                ThrowHelper.ThrowIfDifferentSize(x, y);

                var sum = 0.0;
                var ex = x.GetEnumerator();
                var ey = y.GetEnumerator();
                while (ex.MoveNext())
                {
                    ey.MoveNext();
                    sum += Math.Abs(ex.Current - ey.Current);
                }
                return sum;
            }
        }
    }
}
