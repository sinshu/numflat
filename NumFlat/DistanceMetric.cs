using System;

namespace NumFlat
{
    /// <summary>
    /// Represents a distance measure between two objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the first argument.
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the second argument.
    /// </typeparam>
    /// <param name="x">
    /// The first object to compare.
    /// </param>
    /// <param name="y">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// The computed distance.
    /// </returns>
    public delegate double DistanceMetric<T, U>(T x, U y);



    /// <summary>
    /// Provides distance metrics.
    /// </summary>
    public static class DistanceMetric
    {
        /// <summary>
        /// Gets the Euclidean distance metric.
        /// </summary>
        public static DistanceMetric<Vec<double>, Vec<double>> Euclidean => (Vec<double> x, Vec<double> y) =>
        {
            return x.Distance(y);
        };

        /// <summary>
        /// Gets the Manhattan distance metric.
        /// </summary>
        public static DistanceMetric<Vec<double>, Vec<double>> Manhattan => (Vec<double> x, Vec<double> y) =>
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
        };
    }
}
