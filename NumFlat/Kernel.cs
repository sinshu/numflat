using System;

namespace NumFlat
{
    /// <summary>
    /// Represents a kernel function between two objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the first argument.
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the second argument.
    /// </typeparam>
    public abstract class Kernel<T, U>
    {
        /// <summary>
        /// Computes the kernel value between two objects.
        /// </summary>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// The computed kernel value.
        /// </returns>
        public abstract double Invoke(T x, U y);
    }



    /// <summary>
    /// Represents the linear kernel.
    /// </summary>
    public sealed class LinearKernel : Kernel<Vec<double>, Vec<double>>
    {
        /// <inheritdoc/>
        public override double Invoke(Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            return x * y;
        }
    }



    /// <summary>
    /// Represents a polynomial kernel.
    /// </summary>
    public sealed class PolynomialKernel : Kernel<Vec<double>, Vec<double>>
    {
        private readonly int degree;
        private readonly double constant;

        /// <summary>
        /// Creates a polynomial kernel.
        /// </summary>
        /// <param name="degree">
        /// The degree of the polynomial.
        /// </param>
        /// <param name="constant">
        /// The constant term of the polynomial.
        /// </param>
        public PolynomialKernel(int degree, double constant)
        {
            if (degree <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(degree), "The degree must be greater than zero.");
            }

            this.degree = degree;
            this.constant = constant;
        }

        /// <inheritdoc/>
        public override double Invoke(Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            return Math.Pow(x * y + constant, degree);
        }

        /// <summary>
        /// Gets the degree of the polynomial.
        /// </summary>
        public int Degree => degree;

        /// <summary>
        /// Gets the constant term of the polynomial.
        /// </summary>
        public double Constant => constant;
    }



    /// <summary>
    /// Represents a Gaussian radial basis function (RBF) kernel.
    /// </summary>
    public sealed class GaussianKernel : Kernel<Vec<double>, Vec<double>>
    {
        private readonly double gamma;

        /// <summary>
        /// Creates a Gaussian radial basis function (RBF) kernel.
        /// </summary>
        /// <param name="gamma">
        /// The gamma parameter of the kernel.
        /// </param>
        public GaussianKernel(double gamma)
        {
            if (gamma <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(gamma), "The gamma parameter must be greater than zero.");
            }

            this.gamma = gamma;
        }

        /// <inheritdoc/>
        public override double Invoke(Vec<double> x, Vec<double> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            return Math.Exp(-gamma * x.DistanceSquared(y));
        }

        /// <summary>
        /// Gets the gamma parameter of the kernel.
        /// </summary>
        public double Gamma => gamma;
    }



    /// <summary>
    /// Provides kernel functions.
    /// </summary>
    public static class Kernel
    {
        /// <summary>
        /// Creates a linear kernel.
        /// </summary>
        /// <returns>
        /// The linear kernel.
        /// </returns>
        public static LinearKernel Linear()
        {
            return new LinearKernel();
        }

        /// <summary>
        /// Creates a polynomial kernel.
        /// </summary>
        /// <param name="degree">
        /// The degree of the polynomial.
        /// </param>
        /// <param name="constant">
        /// The constant term of the polynomial.
        /// </param>
        /// <returns>
        /// The polynomial kernel.
        /// </returns>
        public static PolynomialKernel Polynomial(int degree, double constant)
        {
            return new PolynomialKernel(degree, constant);
        }

        /// <summary>
        /// Creates a Gaussian radial basis function (RBF) kernel.
        /// </summary>
        /// <param name="gamma">
        /// The gamma parameter of the kernel.
        /// </param>
        /// <returns>
        /// The Gaussian kernel.
        /// </returns>
        public static GaussianKernel Gaussian(double gamma)
        {
            return new GaussianKernel(gamma);
        }
    }
}
