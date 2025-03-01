using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Performs linear regression.
    /// </summary>
    public sealed class LinearRegression : IVectorToScalarTransform<double>
    {
        private readonly Vec<double> coefficients;
        private readonly double intercept;

        /// <summary>
        /// Performs linear regression.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The target values to be estimated.
        /// </param>
        /// <param name="regularization">
        /// The amount of regularization.
        /// The regularization method is L2 regularization excluding the bias term.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        public LinearRegression(IReadOnlyList<Vec<double>> xs, IReadOnlyList<double> ys, double regularization = 0.0)
        {
            ThrowHelper.ThrowIfNull(xs, nameof(xs));
            ThrowHelper.ThrowIfNull(ys, nameof(ys));

            if (xs.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.", nameof(xs));
            }

            if (xs[0].Count == 0)
            {
                throw new ArgumentException("Empty vectors are not allowed.", nameof(xs));
            }

            if (xs.Count != ys.Count)
            {
                throw new ArgumentException("The number of source vectors and target values must match.");
            }

            if (regularization < 0)
            {
                throw new ArgumentOutOfRangeException("The amount of regularization must be a non-negative value.");
            }

            var d = xs[0].Count;
            var n = xs.Count;

            using var uexpected = new TemporalVector<double>(n);
            ref readonly var expected = ref uexpected.Item;

            using var utmp = new TemporalVector2<double>(d + 1);
            ref readonly var xty = ref utmp.Item1;
            ref readonly var a = ref utmp.Item2;

            using var ubxs = new TemporalMatrix<double>(n, d + 1);
            ref readonly var bxs = ref ubxs.Item;

            using var uxtx = new TemporalMatrix<double>(d + 1, d + 1);
            ref readonly var xtx = ref uxtx.Item;

            // Add the bias term.
            bxs.Cols[0].Fill(1);
            foreach (var (x, row) in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)).Zip(bxs.Cols[1..].Rows))
            {
                x.CopyTo(row);
            }

            // Copy the y values.
            expected.SetInplace(ys);

            // Compute X^T * X.
            Mat.Mul(bxs, bxs, xtx, true, false);

            // Do regularization.
            var i = 0;
            foreach (ref var value in xtx.EnumerateDiagonalElements())
            {
                // Skip the bias term.
                if (i > 0)
                {
                    value += regularization;
                }
                i++;
            }

            // Compute X^T * y.
            Mat.Mul(bxs, expected, xty, true);

            // Compute the coefficients.
            SingularValueDecompositionDouble svd;
            try
            {
                svd = xtx.Svd();
            }
            catch (MatrixFactorizationException e)
            {
                throw new FittingFailureException("Failed to compute the SVD.", e);
            }
            svd.Solve(xty, a);

            this.coefficients = a[1..].Copy();
            this.intercept = a[0];
        }

        /// <inheritdoc/>
        public double Transform(in Vec<double> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            VectorToScalarTransform.ThrowIfInvalidSize(this, source, nameof(source));

            return coefficients * source + intercept;
        }

        /// <summary>
        /// Gets the coefficients.
        /// </summary>
        public ref readonly Vec<double> Coefficients => ref coefficients;

        /// <summary>
        /// Gets the bias term.
        /// </summary>
        public double Intercept => intercept;

        /// <inheritdoc/>
        public int SourceDimension => coefficients.Count;
    }
}
