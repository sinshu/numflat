using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the mean matrix from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="destination">
        /// The destination of the mean matrix.
        /// </param>
        public static void Mean(IEnumerable<Mat<Complex>> xs, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            var count = 0;
            foreach (var x in xs)
            {
                if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                {
                    throw new ArgumentException("All the source matrices must have the same length as the destination.");
                }

                Mat.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one matrix.");
            }

            Mat.Div(destination, count, destination);
        }

        /// <summary>
        /// Computes the mean matrix from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The mean matrix.
        /// </returns>
        public static Mat<Complex> Mean(this IEnumerable<Mat<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Mat<Complex>();

            var count = 0;
            foreach (var x in xs)
            {
                if (destination.RowCount == 0)
                {
                    if (x.RowCount == 0 || x.ColCount == 0)
                    {
                        new ArgumentException("Empty matrices are not allowed.");
                    }

                    destination = new Mat<Complex>(x.RowCount, x.ColCount);
                }

                if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                {
                    throw new ArgumentException("All the matrices must have the same dimensions.");
                }

                Mat.Add(destination, x, destination);
                count++;
            }

            if (count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.");
            }

            Mat.Div(destination, count, destination);

            return destination;
        }
    }
}
