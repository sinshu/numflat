using System;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Provides classical MDS (classical multidimensional scaling).
    /// </summary>
    public static class ClassicalMultiDimensionalScaling
    {
        /// <summary>
        /// Applies classical MDS to the given distance matrix.
        /// </summary>
        /// <param name="distanceMatrix">
        /// The distance matrix.
        /// </param>
        /// <param name="dimension">
        /// The number of dimensions in the embedding space.
        /// </param>
        /// <returns>
        /// The embedded coordinate matrix.
        /// If the distance matrix has dimensions <c>n * n</c>, this matrix has dimensions <c>n * <paramref name="dimension"/></c>,
        /// where the i-th row represents the coordinates of the i-th data point.
        /// </returns>
        public static Mat<double> Fit(in Mat<double> distanceMatrix, int dimension)
        {
            ThrowHelper.ThrowIfEmpty(distanceMatrix, nameof(distanceMatrix));
            ThrowHelper.ThrowIfNonSquare(distanceMatrix, nameof(distanceMatrix));

            if (dimension <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dimension), "The number of dimensions must be greater than zero.");
            }

            var n = distanceMatrix.RowCount;

            if (dimension > n)
            {
                throw new ArgumentException("The number of dimensions must be less than or equal to the size of the distance matrix.");
            }

            using var utmp = new TemporalMatrix4<double>(n, n);
            ref readonly var d2 = ref utmp.Item1;
            ref readonly var j = ref utmp.Item2;
            ref readonly var b = ref utmp.Item3;
            ref readonly var tmp = ref utmp.Item4;

            using var uvec = new TemporalVector<double>(n);
            ref readonly var vec = ref uvec.Item;
            var fvec = vec.GetUnsafeFastIndexer();

            Mat.Map(distanceMatrix, x => x * x, d2);

            j.Fill(-1.0 / n);
            foreach (ref var value in j.EnumerateDiagonalElements())
            {
                value += 1;
            }

            Mat.Mul(j, d2, tmp, false, false);
            Mat.Mul(tmp, j, b, false, false);
            b.MulInplace(-0.5);

            EigenValueDecompositionDouble.Decompose(b, vec, tmp);

            var x = new Mat<double>(n, dimension);
            for (var i = 0; i < dimension; i++)
            {
                Vec.Mul(tmp.Cols[i], Math.Sqrt(fvec[i]), x.Cols[i]);
            }

            return x;
        }

        /// <summary>
        /// Applies classical MDS to the given distance matrix.
        /// </summary>
        /// <param name="distanceMatrix">
        /// The distance matrix.
        /// </param>
        /// <returns>
        /// The embedded coordinate matrix.
        /// If the distance matrix has dimensions <c>n * n</c>, this matrix has dimensions <c>n * n</c>,
        /// where the i-th row represents the coordinates of the i-th data point.
        /// </returns>
        public static Mat<double> Fit(in Mat<double> distanceMatrix)
        {
            return Fit(distanceMatrix, distanceMatrix.RowCount);
        }
    }
}
