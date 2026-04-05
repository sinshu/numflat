using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides non-negative matrix factorization (NMF).
    /// </summary>
    public sealed class NonnegativeMatrixFactorization
    {
        private Mat<double> w;
        private Mat<double> h;

        /// <summary>
        /// Performs non-negative matrix factorization (NMF).
        /// </summary>
        /// <param name="xs">
        /// The source vectors used to form matrix V, where each vector from the list is placed as a column vector in matrix V.
        /// </param>
        /// <param name="componentCount">
        /// The number of basis vectors to be estimated.
        /// </param>
        /// <param name="iterationCount">
        /// The number of iterations to perform for updating the solution.
        /// </param>
        /// <param name="random">
        /// A random number generator for the initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public NonnegativeMatrixFactorization(IReadOnlyList<Vec<double>> xs, int componentCount, int iterationCount = 100, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (componentCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(componentCount), "The number of basis vectors must be greater than zero.");
            }

            if (iterationCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(componentCount), "The number of iterations must be greater than zero.");
            }

            var (w1, h1) = GetInitialGuess(xs, componentCount, random);

            using var uw2 = new TemporalMatrix<double>(w1.RowCount, w1.ColCount);
            ref readonly var w2 = ref uw2.Item;

            using var uh2 = new TemporalMatrix<double>(h1.RowCount, h1.ColCount);
            ref readonly var h2 = ref uh2.Item;

            for (var i = 0; i < iterationCount; i++)
            {
                Update(xs, w1, h1, w2, h2);
                w2.CopyTo(w1);
                h2.CopyTo(h1);
            }

            this.w = w1;
            this.h = h1;
        }

        /// <summary>
        /// Generates initial values for the matrices W and H using a random number generator.
        /// </summary>
        /// <param name="xs">
        /// The source vectors used to form matrix V, where each vector from the list is placed as a column vector in matrix V.
        /// </param>
        /// <param name="componentCount">
        /// The number of basis vectors to be estimated.
        /// </param>
        /// <param name="random">
        /// A random number generator for the initialization.
        /// If null, <see cref="Random.Shared"/> is used.
        /// </param>
        /// <returns>
        /// The randomized matrices W and H.
        /// </returns>
        public static (Mat<double> W, Mat<double> H) GetInitialGuess(IReadOnlyList<Vec<double>> xs, int componentCount, Random? random = null)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));

            if (xs.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.", nameof(xs));
            }

            if (xs[0].Count == 0)
            {
                throw new ArgumentException("Empty vectors are not allowed.", nameof(xs));
            }

            if (random == null)
            {
                random = Random.Shared;
            }

            var w = MatrixBuilder.FromFunc(xs[0].Count, componentCount, (row, col) => random.NextDouble());
            var h = MatrixBuilder.FromFunc(componentCount, xs.Count, (row, col) => random.NextDouble());
            return (w, h);
        }

        /// <summary>
        /// Updates the matrices W and H using coordinate descent for NMF.
        /// </summary>
        /// <param name="xs">
        /// The source vectors used to form matrix V, where each vector from the list is placed as a column vector in matrix V.
        /// </param>
        /// <param name="sourceW">
        /// The source matrix W, containing basis vectors as columns.
        /// </param>
        /// <param name="sourceH">
        /// The source matrix H, containing activation vectors as columns.
        /// </param>
        /// <param name="destinationW">
        /// The destination matrix where the updated W will be stored.
        /// </param>
        /// <param name="destinationH">
        /// The destination matrix where the updated H will be stored.
        /// </param>
        public static void Update(IReadOnlyList<Vec<double>> xs, in Mat<double> sourceW, in Mat<double> sourceH, in Mat<double> destinationW, in Mat<double> destinationH)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfEmpty(sourceW, nameof(sourceW));
            ThrowHelper.ThrowIfEmpty(sourceH, nameof(sourceH));
            ThrowHelper.ThrowIfEmpty(destinationW, nameof(destinationW));
            ThrowHelper.ThrowIfEmpty(destinationH, nameof(destinationH));

            if (sourceW.RowCount != destinationW.RowCount || sourceW.ColCount != destinationW.ColCount)
            {
                throw new ArgumentException("The dimensions of 'sourceW' and 'destinationW' must match.");
            }

            if (sourceH.RowCount != destinationH.RowCount || sourceH.ColCount != destinationH.ColCount)
            {
                throw new ArgumentException("The dimensions of 'sourceH' and 'destinationH' must match.");
            }

            if (sourceW.ColCount != sourceH.RowCount)
            {
                throw new ArgumentException("'sourceW.ColCount' and 'sourceH.RowCount' must match.");
            }

            if (sourceH.ColCount != xs.Count)
            {
                throw new ArgumentException("'sourceH.ColCount' and 'xs.Count' must match.");
            }

            var dimension = sourceW.RowCount;
            var componentCount = sourceW.ColCount;
            var dataCount = xs.Count;
            var v = xs.ThrowIfEmptyOrDifferentSize(dimension, nameof(xs));

            sourceW.CopyTo(destinationW);
            sourceH.CopyTo(destinationH);

            using var ugram = new TemporalMatrix<double>(componentCount, componentCount);
            ref readonly var gram = ref ugram.Item;

            using var uxht = new TemporalMatrix2<double>(dimension, componentCount);
            ref readonly var xht = ref uxht.Item1;
            ref readonly var outer = ref uxht.Item2;

            xht.Clear();
            foreach (var (x, col) in v.Zip(sourceH.Cols))
            {
                Vec.Outer(x, col, outer);
                xht.AddInplace(outer);
            }
            Mat.Mul(sourceH, sourceH, gram, false, true);
            UpdateCoordinateDescent(destinationW, gram, xht);

            using var uwtx = new TemporalMatrix<double>(componentCount, dataCount);
            ref readonly var wtx = ref uwtx.Item;

            foreach (var (x, col) in v.Zip(wtx.Cols))
            {
                Mat.Mul(destinationW, x, col, true);
            }
            Mat.Mul(destinationW, destinationW, gram, true, false);
            UpdateCoordinateDescentTransposed(destinationH, gram, wtx);
        }

        private static void UpdateCoordinateDescent(in Mat<double> factor, in Mat<double> gram, in Mat<double> cross)
        {
            var ffactor = factor.GetUnsafeFastIndexer();
            var fgram = gram.GetUnsafeFastIndexer();
            var fcross = cross.GetUnsafeFastIndexer();

            for (var t = 0; t < factor.ColCount; t++)
            {
                var hess = fgram[t, t];
                if (hess == 0)
                {
                    continue;
                }

                for (var i = 0; i < factor.RowCount; i++)
                {
                    var grad = -fcross[i, t];
                    for (var r = 0; r < factor.ColCount; r++)
                    {
                        grad += fgram[t, r] * ffactor[i, r];
                    }

                    var updated = ffactor[i, t] - (grad / hess);
                    ffactor[i, t] = updated > 0 ? updated : 0;
                }
            }
        }

        private static void UpdateCoordinateDescentTransposed(in Mat<double> factor, in Mat<double> gram, in Mat<double> cross)
        {
            var ffactor = factor.GetUnsafeFastIndexer();
            var fgram = gram.GetUnsafeFastIndexer();
            var fcross = cross.GetUnsafeFastIndexer();

            for (var t = 0; t < factor.RowCount; t++)
            {
                var hess = fgram[t, t];
                if (hess == 0)
                {
                    continue;
                }

                for (var i = 0; i < factor.ColCount; i++)
                {
                    var grad = -fcross[t, i];
                    for (var r = 0; r < factor.RowCount; r++)
                    {
                        grad += fgram[t, r] * ffactor[r, i];
                    }

                    var updated = ffactor[t, i] - (grad / hess);
                    ffactor[t, i] = updated > 0 ? updated : 0;
                }
            }
        }

        /// <summary>
        /// Gets the basis vectors.
        /// </summary>
        public ref readonly Mat<double> W => ref w;

        /// <summary>
        /// Gets the activation vectors.
        /// </summary>
        public ref readonly Mat<double> H => ref h;
    }
}
