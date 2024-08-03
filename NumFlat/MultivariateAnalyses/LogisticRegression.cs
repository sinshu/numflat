﻿using System;
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
        /// <exception cref="FittingFailureException">
        /// Failed to fit the model.
        /// </exception>
        /// <remarks>
        /// This logistic regression implementation assumes two classes.
        /// Therefore, only 0 or 1 are valid as class indices.
        /// </remarks>
        public LogisticRegression(IReadOnlyList<Vec<double>> xs, IEnumerable<int> ys)
        {
            var n = xs.Count;
            var d = xs[0].Count + 1;

            var betas = new Vec<double>(d);
            var y = new Vec<double>(n);
            var grad = new Vec<double>(d);

            var trainX = new Mat<double>(n, d);
            foreach (var (x, row) in xs.Zip(trainX.Cols[0..xs[0].Count].Rows))
            {
                x.CopyTo(row);
            }
            trainX.Cols.Last().Fill(1);

            var trainY = ys.Select(value => (double)value).ToVector();

            for (var i = 0; i < 10; i++)
            {
                // y = self.sigmoid(np.matmul(self.train_X,betas))
                Mat.Mul(trainX, betas, y, false);
                y.MapInplace(Special.Sigmoid);

                // R = np.diag(np.ravel(y*(1-y)))
                var diag = y.Map(val => val * (1 - val));
                var r = diag.ToDiagonalMatrix();

                // grad = np.matmul(self.train_X.T,(y-self.train_y))
                Mat.Mul(trainX, y - trainY, grad, true);

                // hessian = np.matmul(np.matmul(self.train_X.T,R),self.train_X)+0.001*np.eye(self.D)
                var tmp = trainX.Transpose() * r;
                var hessian = tmp * trainX + 0.001 * MatrixBuilder.Identity<double>(d);

                // betas -= np.matmul(inv(hessian),grad)
                betas -= hessian.Inverse() * grad;

                //Console.WriteLine(betas);
                //Console.WriteLine(y);
            }

            foreach (var (x, label) in xs.Zip(ys))
            {
                //var value = Special.Sigmoid(x.Append(1).ToVector() * betas);
                //Console.WriteLine(label + ", " + value);
            }

            coefficients = betas[0..(d - 1)];
            intercept = betas.Last();
        }

        /// <inheritdoc/>
        public double Transform(in Vec<double> source)
        {
            return Special.Sigmoid(coefficients * source + intercept);
        }

        /// <inheritdoc/>
        public int Predict(in Vec<double> x)
        {
            if (Transform(x) < 0.5)
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
            destination[0] = 1 - value;
            destination[1] = value;
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
