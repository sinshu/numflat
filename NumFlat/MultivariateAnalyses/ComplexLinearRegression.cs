using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Performs complex linear regression.
    /// </summary>
    public sealed class ComplexLinearRegression : IVectorToScalarTransform<Complex>
    {
        private readonly Vec<Complex> coefficients;
        private readonly Complex intercept;

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
        /// <remarks>
        /// Note that this implementation assumes a regression problem of the form <c>y = w^H x + b</c>,
        /// where <c>x</c> is the input vector, <c>w</c> is the coefficient vector, and <c>b</c> is the bias term.
        /// In other words, to transform the input vector using the estimated coefficient vector,
        /// it is necessary to take the Hermitian transpose of the coefficient vector.
        /// </remarks>
        public ComplexLinearRegression(IReadOnlyList<Vec<Complex>> xs, IReadOnlyList<Complex> ys, double regularization = 0.0)
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

            using var uexpected = new TemporalVector<Complex>(n);
            ref readonly var expected = ref uexpected.Item;

            using var utmp = new TemporalVector2<Complex>(d + 1);
            ref readonly var xhy = ref utmp.Item1;
            ref readonly var a = ref utmp.Item2;

            using var ubxs = new TemporalMatrix<Complex>(n, d + 1);
            ref readonly var bxs = ref ubxs.Item;

            using var uxhx = new TemporalMatrix<Complex>(d + 1, d + 1);
            ref readonly var xhx = ref uxhx.Item;

            // Add the bias term.
            bxs.Cols[0].Fill(1);
            foreach (var (x, row) in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)).Zip(bxs.Cols[1..].Rows))
            {
                x.CopyTo(row);
            }

            // Copy the y values.
            expected.SetInplace(ys);

            // Compute X^H * X.
            Mat.Mul(bxs, bxs, xhx, true, true, false, false);

            // Do regularization.
            var i = 0;
            foreach (ref var value in xhx.EnumerateDiagonalElements())
            {
                // Skip the bias term.
                if (i > 0)
                {
                    value += regularization;
                }
                i++;
            }

            // Compute X^H * y.
            Mat.Mul(bxs, expected, xhy, true, true);

            // Compute the coefficients.
            SingularValueDecompositionComplex svd;
            try
            {
                svd = xhx.Svd();
            }
            catch (MatrixFactorizationException e)
            {
                throw new FittingFailureException("Failed to compute the SVD.", e);
            }
            svd.Solve(xhy, a);

            this.coefficients = a[1..].Conjugate();
            this.intercept = a[0];
        }

        /// <inheritdoc/>
        public Complex Transform(in Vec<Complex> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            VectorToScalarTransform.ThrowIfInvalidSize(this, source, nameof(source));

            return Vec.Dot(coefficients, source, true) + intercept;
        }

        /// <summary>
        /// Gets the coefficients.
        /// </summary>
        public ref readonly Vec<Complex> Coefficients => ref coefficients;

        /// <summary>
        /// Gets the bias term.
        /// </summary>
        public Complex Intercept => intercept;

        /// <inheritdoc/>
        public int SourceDimension => coefficients.Count;
    }
}
