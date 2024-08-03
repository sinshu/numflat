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
                throw new ArgumentException("The class indices must include both 0 and 1, and it must not contain any other values.", nameof(ys));
            }

            if (options == null)
            {
                options = new LogisticRegressionOptions();
            }

            var d = xs[0].Count;

            using var ua = new TemporalVector3<double>(d + 1);
            ref readonly var a = ref ua.Item1;
            ref readonly var gradient = ref ua.Item2;
            ref readonly var delta = ref ua.Item3;

            using var utmpv = new TemporalVector4<double>(xs.Count);
            ref readonly var reference = ref utmpv.Item1;
            ref readonly var prediction = ref utmpv.Item2;
            ref readonly var error = ref utmpv.Item3;
            ref readonly var rs = ref utmpv.Item4;

            using var ux1 = new TemporalMatrix2<double>(xs.Count, d + 1);
            ref readonly var x1s = ref ux1.Item1;
            ref readonly var x1rs = ref ux1.Item2;

            using var uhessian = new TemporalMatrix<double>(d + 1, d + 1);
            ref readonly var hessian = ref uhessian.Item;

            var prev = 100.0;

            foreach (var (x, row) in xs.Zip(x1s.Cols[0..d].Rows))
            {
                x.CopyTo(row);
            }
            x1s.Cols.Last().Fill(1);

            //var trainY = ys.Select(value => (double)value).ToVector();
            using (var enumerator = ys.GetEnumerator())
            {
                foreach (ref var value in reference)
                {
                    enumerator.MoveNext();
                    value = enumerator.Current;
                }
            }

            a.Clear();

            for (var iter = 0; iter < options.MaxIterations; iter++)
            {
                // y = self.sigmoid(np.matmul(self.train_X,betas))
                Mat.Mul(x1s, a, prediction, false);
                prediction.MapInplace(Special.Sigmoid);

                // R = np.diag(np.ravel(y*(1-y)))
                Vec.Map(prediction, val => val * (1 - val), rs);
                //var r = diag.ToDiagonalMatrix();

                // grad = np.matmul(self.train_X.T,(y-self.train_y))
                Vec.Sub(prediction, reference, error);
                Mat.Mul(x1s, error, gradient, true);

                foreach (var (x1, r, x1r) in x1s.Rows.Zip(rs, x1rs.Rows))
                {
                    Vec.Mul(x1, r, x1r);
                }

                // hessian = np.matmul(np.matmul(self.train_X.T,R),self.train_X)+0.001*np.eye(self.D)
                //var tmp = x1s.Transpose() * r;
                //var tmp = x1rs.Transpose();
                //var hessian = tmp * x1s + 0.001 * MatrixBuilder.Identity<double>(d);
                Mat.Mul(x1rs, x1s, hessian, true, false);
                var i = 0;
                foreach (ref var value in hessian.EnumerateDiagonalElements())
                {
                    if (i < d)
                    {
                        value += options.Regularization;
                    }
                    else
                    {
                        // Skip the bias term.
                        break;
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
                var curr = delta.Select(Math.Abs).Max();
                Console.WriteLine(curr);
                prev = curr;
            }

            this.coefficients = new Vec<double>(xs[0].Count);
            a[0..xs[0].Count].CopyTo(this.coefficients);

            this.intercept = a.Last();
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
