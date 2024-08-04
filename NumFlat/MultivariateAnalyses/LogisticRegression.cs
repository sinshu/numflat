using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    /// <summary>
    /// Performs logistic regression.
    /// </summary>
    public sealed class LogisticRegression : IVectorToScalarTransform<double>, IProbabilisticClassifier<double>
    {
        private readonly Vec<double> coefficients;
        private readonly double intercept;

        /// <summary>
        /// Performs logistic regression.
        /// </summary>
        /// <param name="xs">
        /// The source vectors.
        /// </param>
        /// <param name="ys">
        /// The class indices for each source vector.
        /// </param>
        /// <param name="options">
        /// Specifies options for logistic regression.
        /// </param>
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// This logistic regression implementation assumes two classes.
        /// Therefore, only 0 or 1 are valid as class indices.
        /// </remarks>
        public LogisticRegression(IReadOnlyList<Vec<double>> xs, IReadOnlyList<int> ys, LogisticRegressionOptions? options = null)
        {
            //
            // This is based on the following implementation.
            // https://github.com/jaeho3690/LogisiticRegression/tree/main
            //

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
                throw new ArgumentException("The number of source vectors and class indices must match.");
            }

            if (ys.Min() != 0 || ys.Max() != 1)
            {
                if (ys.All(y => y == 0) || ys.All(y => y == 1))
                {
                    throw new ArgumentException("All class indices have the same value. The class indices must include both 0 and 1.", nameof(ys));
                }
                else
                {
                    throw new ArgumentException("The class indices must be either 0 or 1.", nameof(ys));
                }
            }

            if (options == null)
            {
                options = new LogisticRegressionOptions();
            }

            var d = xs[0].Count;
            var n = xs.Count;

            using var ua = new TemporalVector3<double>(d + 1);
            ref readonly var a = ref ua.Item1;
            ref readonly var gradient = ref ua.Item2;
            ref readonly var delta = ref ua.Item3;

            using var utmpv = new TemporalVector4<double>(n);
            ref readonly var reference = ref utmpv.Item1;
            ref readonly var prediction = ref utmpv.Item2;
            ref readonly var error = ref utmpv.Item3;
            ref readonly var weights = ref utmpv.Item4;

            using var ux1 = new TemporalMatrix2<double>(n, d + 1);
            ref readonly var bxs = ref ux1.Item1;
            ref readonly var bxws = ref ux1.Item2;

            using var uhessian = new TemporalMatrix<double>(d + 1, d + 1);
            ref readonly var hessian = ref uhessian.Item;

            // Add bias term.
            bxs.Cols[0].Fill(1);
            foreach (var (x, row) in xs.ThrowIfEmptyOrDifferentSize(nameof(xs)).Zip(bxs.Cols[1..].Rows))
            {
                x.CopyTo(row);
            }

            using (var enumerator = ys.GetEnumerator())
            {
                foreach (ref var value in reference)
                {
                    enumerator.MoveNext();
                    value = enumerator.Current;
                }
            }

            a.Clear();

            var prevLoss = double.MaxValue;

            for (var iter = 0; iter < options.MaxIterations; iter++)
            {
                Mat.Mul(bxs, a, prediction, false);
                prediction.MapInplace(Special.Sigmoid);

                Vec.Map(prediction, val => val * (1 - val), weights);

                Vec.Sub(prediction, reference, error);
                Mat.Mul(bxs, error, gradient, true);

                foreach (var (bx, w, bxw) in bxs.Rows.Zip(weights, bxws.Rows))
                {
                    Vec.Mul(bx, w, bxw);
                }

                Mat.Mul(bxws, bxs, hessian, true, false);
                var i = 0;
                foreach (ref var value in hessian.EnumerateDiagonalElements())
                {
                    // Skip the bias term.
                    if (i > 0)
                    {
                        value += options.Regularization;
                    }
                    i++;
                }

                SingularValueDecompositionDouble svd;
                try
                {
                    svd = hessian.Svd();
                }
                catch (MatrixFactorizationException e)
                {
                    throw new FittingFailureException("Failed to compute the SVD of the Hessian matrix.", e);
                }
                svd.Solve(gradient, delta);
                a.SubInplace(delta);

                var currLoss = CrossEntropyLoss(reference, prediction, 1.0E-10);
                if (Math.Abs(currLoss - prevLoss) <= options.Tolerance)
                {
                    break;
                }
                prevLoss = currLoss;
            }

            this.coefficients = new Vec<double>(d);
            a[1..].CopyTo(this.coefficients);

            this.intercept = a[0];
        }

        private static double CrossEntropyLoss(in Vec<double> reference, in Vec<double> prediction, double eps)
        {
            var sum = 0.0;
            foreach (var (y1, y2) in reference.Zip(prediction))
            {
                var clipped = Math.Clamp(y2, eps, 1 - eps);
                sum += y1 * Math.Log(clipped) + (1 - y1) * Math.Log(1 - clipped);
            }
            return -sum;
        }

        /// <inheritdoc/>
        public double Transform(in Vec<double> source)
        {
            ThrowHelper.ThrowIfEmpty(source, nameof(source));
            VectorToScalarTransform.ThrowIfInvalidSize(this, source, nameof(source));

            return Special.Sigmoid(coefficients * source + intercept);
        }

        /// <inheritdoc/>
        public int Predict(in Vec<double> x)
        {
            var value = Transform(x);

            if (value < 0.5)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <inheritdoc/>
        public void PredictProbability(in Vec<double> x, in Vec<double> destination)
        {
            var value = Transform(x);

            var fd = destination.GetUnsafeFastIndexer();
            fd[0] = 1 - value;
            fd[1] = value;
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

        /// <inheritdoc/>
        public int Dimension => coefficients.Count;

        /// <inheritdoc/>
        public int ClassCount => 2;
    }
}
