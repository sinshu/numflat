using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace NumFlat
{
    public partial struct Mat<T>
    {
        /// <summary>
        /// Computes a matrix addition, X + Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <returns>
        /// The result of the matrix addition.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Add{T}(in Mat{T}, in Mat{T}, in Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> operator +(in Mat<T> x, in Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var result = new Mat<T>(x.rowCount, x.colCount);
            Mat.Add(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix subtraction, X - Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <returns>
        /// The result of the matrix subtraction.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Sub{T}(in Mat{T}, in Mat{T}, in Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> operator -(in Mat<T> x, in Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));
            ThrowHelper.ThrowIfDifferentSize(x, y);

            var result = new Mat<T>(x.rowCount, x.colCount);
            Mat.Sub(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix-and-scalar multiplication, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The result of the matrix-and-scalar multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Mul{T}(in Mat{T}, T, in Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> operator *(in Mat<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<T>(x.rowCount, x.colCount);
            Mat.Mul(x, y, result);
            return result;
        }

        /// <summary>
        /// Computes a scalar-and-matrix multiplication, x * Y.
        /// </summary>
        /// <param name="x">
        /// The scalar X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <returns>
        /// The result of the scalar-and-matri multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Mul{T}(in Mat{T}, T, in Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> operator *(T x, in Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

            var result = new Mat<T>(y.rowCount, y.colCount);
            Mat.Mul(y, x, result);
            return result;
        }

        /// <summary>
        /// Computes a matrix-and-vector multiplication, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The vector y.
        /// This vector is interpreted as a column vector.
        /// </param>
        /// <returns>
        /// The result of the matrix-and-vector multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new vector which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Mul(in Mat{double}, in Mat{double}, in Mat{double}, bool, bool)"/> instead.
        /// </remarks>
        public static Vec<T> operator *(Mat<T> x, Vec<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

            if (y.Count != x.ColCount)
            {
                throw new ArgumentException("'y.Count' must match 'x.ColCount'.");
            }

            if (typeof(T) == typeof(float))
            {
                var ux = Unsafe.As<Mat<T>, Mat<float>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<float>>(ref y);
                var result = new Vec<float>(x.rowCount);
                Mat.Mul(ux, uy, result, false);
                return Unsafe.As<Vec<float>, Vec<T>>(ref result);
            }
            else if (typeof(T) == typeof(double))
            {
                var ux = Unsafe.As<Mat<T>, Mat<double>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<double>>(ref y);
                var result = new Vec<double>(x.rowCount);
                Mat.Mul(ux, uy, result, false);
                return Unsafe.As<Vec<double>, Vec<T>>(ref result);
            }
            else if (typeof(T) == typeof(Complex))
            {
                var ux = Unsafe.As<Mat<T>, Mat<Complex>>(ref x);
                var uy = Unsafe.As<Vec<T>, Vec<Complex>>(ref y);
                var result = new Vec<Complex>(x.rowCount);
                Mat.Mul(ux, uy, result, false, false);
                return Unsafe.As<Vec<Complex>, Vec<T>>(ref result);
            }
            else
            {
                throw new NotSupportedException($"Matrix multiplication for the type '{typeof(T).Name}' is not supported.");
            }
        }

        /// <summary>
        /// Computes a matrix multiplication, X * Y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The matrix Y.
        /// </param>
        /// <returns>
        /// The result of the matrix multiplication.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Mul(in Mat{double}, in Mat{double}, in Mat{double}, bool, bool)"/> instead.
        /// </remarks>
        public static Mat<T> operator *(Mat<T> x, Mat<T> y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));
            ThrowHelper.ThrowIfEmpty(y, nameof(y));

            if (x.colCount != y.rowCount)
            {
                throw new ArgumentException("'y.RowCount' must match 'x.ColCount'.");
            }

            if (typeof(T) == typeof(float))
            {
                var ux = Unsafe.As<Mat<T>, Mat<float>>(ref x);
                var uy = Unsafe.As<Mat<T>, Mat<float>>(ref y);
                var result = new Mat<float>(x.rowCount, y.colCount);
                Mat.Mul(ux, uy, result, false, false);
                return Unsafe.As<Mat<float>, Mat<T>>(ref result);
            }
            else if (typeof(T) == typeof(double))
            {
                var ux = Unsafe.As<Mat<T>, Mat<double>>(ref x);
                var uy = Unsafe.As<Mat<T>, Mat<double>>(ref y);
                var result = new Mat<double>(x.rowCount, y.colCount);
                Mat.Mul(ux, uy, result, false, false);
                return Unsafe.As<Mat<double>, Mat<T>>(ref result);
            }
            else if (typeof(T) == typeof(Complex))
            {
                var ux = Unsafe.As<Mat<T>, Mat<Complex>>(ref x);
                var uy = Unsafe.As<Mat<T>, Mat<Complex>>(ref y);
                var result = new Mat<Complex>(x.rowCount, y.colCount);
                Mat.Mul(ux, uy, result, false, false, false, false);
                return Unsafe.As<Mat<Complex>, Mat<T>>(ref result);
            }
            else
            {
                throw new NotSupportedException($"Matrix multiplication for the type '{typeof(T).Name}' is not supported.");
            }
        }

        /// <summary>
        /// Computes a matrix-and-scalar division, X * y.
        /// </summary>
        /// <param name="x">
        /// The matrix X.
        /// </param>
        /// <param name="y">
        /// The scalar y.
        /// </param>
        /// <returns>
        /// The result of the matrix-and-scalar division.
        /// </returns>
        /// <remarks>
        /// This method allocates a new matrix which is independent from the original matrices.
        /// To avoid the allocation, use <see cref="Mat.Div{T}(in Mat{T}, T, in Mat{T})"/> instead.
        /// </remarks>
        public static Mat<T> operator /(in Mat<T> x, T y)
        {
            ThrowHelper.ThrowIfEmpty(x, nameof(x));

            var result = new Mat<T>(x.rowCount, x.colCount);
            Mat.Div(x, y, result);
            return result;
        }
    }
}
