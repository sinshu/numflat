using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    public static partial class MathLinq
    {
        /// <summary>
        /// Computes the matrix sum from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="destination">
        /// The destination of the matrix sum.
        /// </param>
        public static void Sum(IEnumerable<Mat<Complex>> xs, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            foreach (var x in xs)
            {
                if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                {
                    throw new ArgumentException("All the source matrices must have the same dimensions as the destination.");
                }

                destination.AddInplace(x);
            }
        }

        /// <summary>
        /// Computes the matrix sum from a sequence of matrices.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <returns>
        /// The matrix sum.
        /// </returns>
        public static Mat<Complex> Sum(this IEnumerable<Mat<Complex>> xs)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            var destination = new Mat<Complex>();

            foreach (var x in xs)
            {
                if (destination.RowCount == 0)
                {
                    if (x.RowCount == 0 || x.ColCount == 0)
                    {
                        throw new ArgumentException("Empty matrices are not allowed.");
                    }

                    destination = new Mat<Complex>(x.RowCount, x.ColCount);
                }

                if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                {
                    throw new ArgumentException("All the matrices must have the same dimensions.");
                }

                destination.AddInplace(x);
            }

            if (destination.RowCount == 0)
            {
                throw new ArgumentException("The sequence must contain at least one matrix.");
            }

            return destination;
        }

        /// <summary>
        /// Computes the matrix sum from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <param name="destination">
        /// The destination of the weighted matrix sum.
        /// </param>
        public static void Sum(IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights, in Mat<Complex> destination)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));
            ThrowHelper.ThrowIfEmpty(destination, nameof(destination));

            destination.Clear();

            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {
                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var weightsHasNext = eweights.MoveNext();
                    if (xsHasNext != weightsHasNext)
                    {
                        throw new ArgumentException("The number of source matrices and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                        {
                            throw new ArgumentException("All the source matrices must have the same dimensions as the destination.");
                        }

                        AccumulateWeightedSum(x, w, destination);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Computes the matrix sum from a sequence of matrices and their weights.
        /// </summary>
        /// <param name="xs">
        /// The source matrices.
        /// </param>
        /// <param name="weights">
        /// The weights of the source matrices.
        /// </param>
        /// <returns>
        /// The weighted matrix sum.
        /// </returns>
        public static Mat<Complex> Sum(this IEnumerable<Mat<Complex>> xs, IEnumerable<double> weights)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(weights, nameof(weights));

            var destination = new Mat<Complex>();

            using (var exs = xs.GetEnumerator())
            using (var eweights = weights.GetEnumerator())
            {

                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var weightsHasNext = eweights.MoveNext();
                    if (xsHasNext != weightsHasNext)
                    {
                        throw new ArgumentException("The number of matrices and weights must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var w = eweights.Current;

                        if (destination.RowCount == 0)
                        {
                            if (x.RowCount == 0 || x.ColCount == 0)
                            {
                                throw new ArgumentException("Empty matrices are not allowed.");
                            }

                            destination = new Mat<Complex>(x.RowCount, x.ColCount);
                        }

                        if (x.RowCount != destination.RowCount || x.ColCount != destination.ColCount)
                        {
                            throw new ArgumentException("All the matrices must have the same dimensions.");
                        }

                        AccumulateWeightedSum(x, w, destination);
                    }
                    else
                    {
                        break;
                    }
                }

            }

            if (destination.RowCount == 0)
            {
                throw new ArgumentException("The sequence must contain at least one matrix.");
            }

            return destination;
        }
    }
}
