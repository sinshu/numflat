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
        private const double MinimumValue = 1.0E-12;
        private const double UpdateTolerance = 1.0E-12;

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
        /// Updates the matrices W and H using greedy coordinate descent for NMF.
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
            var validatedXs = xs.ThrowIfEmptyOrDifferentSize(dimension, nameof(xs)).ToArray();

            sourceW.CopyTo(destinationW);
            sourceH.CopyTo(destinationH);

            UpdateActivationMatrix(validatedXs, sourceW, destinationH);
            UpdateBasisMatrix(validatedXs, destinationH, destinationW);
        }

        private static void UpdateActivationMatrix(IReadOnlyList<Vec<double>> xs, in Mat<double> w, in Mat<double> h)
        {
            var dimension = w.RowCount;
            var componentCount = w.ColCount;
            var dataCount = xs.Count;

            using var unorms = new TemporalVector<double>(componentCount);
            ref readonly var wNorms = ref unorms.Item;

            var wNormsIndexer = new Vec<double>.UnsafeFastIndexer(wNorms);
            for (var k = 0; k < componentCount; k++)
            {
                wNormsIndexer[k] = Math.Max(Vec.Dot(w.Cols[k], w.Cols[k]), MinimumValue);
            }

            using var uresidual = new TemporalVector<double>(dimension);
            ref readonly var residual = ref uresidual.Item;
            var residualIndexer = new Vec<double>.UnsafeFastIndexer(residual);
            var hIndexer = new Mat<double>.UnsafeFastIndexer(h);

            for (var sampleIndex = 0; sampleIndex < dataCount; sampleIndex++)
            {
                var x = xs[sampleIndex];

                for (var row = 0; row < dimension; row++)
                {
                    residualIndexer[row] = -x[row];
                }

                for (var k = 0; k < componentCount; k++)
                {
                    var coefficient = hIndexer[k, sampleIndex];
                    if (coefficient <= MinimumValue)
                    {
                        continue;
                    }

                    var basis = w.Cols[k];
                    for (var row = 0; row < dimension; row++)
                    {
                        residualIndexer[row] += coefficient * basis[row];
                    }
                }

                for (var updateIndex = 0; updateIndex < componentCount; updateIndex++)
                {
                    var bestComponent = -1;
                    var bestValue = 0.0;
                    var bestDelta = 0.0;
                    var bestScore = 0.0;

                    for (var k = 0; k < componentCount; k++)
                    {
                        var basis = w.Cols[k];
                        var gradient = Vec.Dot(basis, residual);
                        var current = hIndexer[k, sampleIndex];
                        var projectedGradient = current > MinimumValue ? gradient : Math.Min(gradient, 0.0);
                        var score = Math.Abs(projectedGradient);
                        if (score <= bestScore + UpdateTolerance)
                        {
                            continue;
                        }

                        var updated = Math.Max(current - (gradient / wNormsIndexer[k]), 0.0);
                        var delta = updated - current;
                        if (Math.Abs(delta) <= UpdateTolerance)
                        {
                            continue;
                        }

                        bestComponent = k;
                        bestValue = updated;
                        bestDelta = delta;
                        bestScore = score;
                    }

                    if (bestComponent < 0)
                    {
                        break;
                    }

                    hIndexer[bestComponent, sampleIndex] = bestValue;
                    var bestBasis = w.Cols[bestComponent];
                    for (var row = 0; row < dimension; row++)
                    {
                        residualIndexer[row] += bestDelta * bestBasis[row];
                    }
                }
            }
        }

        private static void UpdateBasisMatrix(IReadOnlyList<Vec<double>> xs, in Mat<double> h, in Mat<double> w)
        {
            var dimension = w.RowCount;
            var componentCount = w.ColCount;
            var dataCount = xs.Count;

            using var unorms = new TemporalVector<double>(componentCount);
            ref readonly var hNorms = ref unorms.Item;

            var hNormsIndexer = new Vec<double>.UnsafeFastIndexer(hNorms);
            for (var k = 0; k < componentCount; k++)
            {
                hNormsIndexer[k] = Math.Max(Vec.Dot(h.Rows[k], h.Rows[k]), MinimumValue);
            }

            using var uresidual = new TemporalVector<double>(dataCount);
            ref readonly var residual = ref uresidual.Item;
            var residualIndexer = new Vec<double>.UnsafeFastIndexer(residual);
            var wIndexer = new Mat<double>.UnsafeFastIndexer(w);

            for (var rowIndex = 0; rowIndex < dimension; rowIndex++)
            {
                for (var sampleIndex = 0; sampleIndex < dataCount; sampleIndex++)
                {
                    residualIndexer[sampleIndex] = -xs[sampleIndex][rowIndex];
                }

                for (var k = 0; k < componentCount; k++)
                {
                    var coefficient = wIndexer[rowIndex, k];
                    if (coefficient <= MinimumValue)
                    {
                        continue;
                    }

                    var activation = h.Rows[k];
                    for (var sampleIndex = 0; sampleIndex < dataCount; sampleIndex++)
                    {
                        residualIndexer[sampleIndex] += coefficient * activation[sampleIndex];
                    }
                }

                for (var updateIndex = 0; updateIndex < componentCount; updateIndex++)
                {
                    var bestComponent = -1;
                    var bestValue = 0.0;
                    var bestDelta = 0.0;
                    var bestScore = 0.0;

                    for (var k = 0; k < componentCount; k++)
                    {
                        var activation = h.Rows[k];
                        var gradient = Vec.Dot(residual, activation);
                        var current = wIndexer[rowIndex, k];
                        var projectedGradient = current > MinimumValue ? gradient : Math.Min(gradient, 0.0);
                        var score = Math.Abs(projectedGradient);
                        if (score <= bestScore + UpdateTolerance)
                        {
                            continue;
                        }

                        var updated = Math.Max(current - (gradient / hNormsIndexer[k]), 0.0);
                        var delta = updated - current;
                        if (Math.Abs(delta) <= UpdateTolerance)
                        {
                            continue;
                        }

                        bestComponent = k;
                        bestValue = updated;
                        bestDelta = delta;
                        bestScore = score;
                    }

                    if (bestComponent < 0)
                    {
                        break;
                    }

                    wIndexer[rowIndex, bestComponent] = bestValue;
                    var bestActivation = h.Rows[bestComponent];
                    for (var sampleIndex = 0; sampleIndex < dataCount; sampleIndex++)
                    {
                        residualIndexer[sampleIndex] += bestDelta * bestActivation[sampleIndex];
                    }
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
