using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides in-place operations for <see cref="Mat{T}"/>.
    /// </summary>
    public static class MatrixInplaceOperations
    {
        /// <summary>
        /// Computes a matrix addition in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be added.
        /// </param>
        /// <param name="x">
        /// The matrix to add.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void AddInplace<T>(in this Mat<T> target, in Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            Mat.Add(target, x, target);
        }

        /// <summary>
        /// Computes a pointwise matrix-and-scalar addition in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be added.
        /// </param>
        /// <param name="x">
        /// The scalar to add.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void AddInplace<T>(in this Mat<T> target, T x) where T : unmanaged, INumberBase<T>
        {
            Mat.Add(target, x, target);
        }

        /// <summary>
        /// Computes a matrix subtraction in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be subtracted.
        /// </param>
        /// <param name="x">
        /// The matrix to sbtract.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void SubInplace<T>(in this Mat<T> target, in Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            Mat.Sub(target, x, target);
        }

        /// <summary>
        /// Computes a pointwise matrix-and-scalar subtraction in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be subtracted.
        /// </param>
        /// <param name="x">
        /// The scalar to sbtract.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void SubInplace<T>(in this Mat<T> target, T x) where T : unmanaged, INumberBase<T>
        {
            Mat.Sub(target, x, target);
        }

        /// <summary>
        /// Computes a matrix-and-scalar multiplication in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be multiplied.
        /// </param>
        /// <param name="x">
        /// The scalar to multiply.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void MulInplace<T>(in this Mat<T> target, T x) where T : unmanaged, INumberBase<T>
        {
            Mat.Mul(target, x, target);
        }

        /// <summary>
        /// Computes a matrix-and-scalar division in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be divided.
        /// </param>
        /// <param name="x">
        /// The scalar to divide.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void DivInplace<T>(in this Mat<T> target, T x) where T : unmanaged, INumberBase<T>
        {
            Mat.Div(target, x, target);
        }

        /// <summary>
        /// Computes a matrix pointwise multiplication in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be multiplied.
        /// </param>
        /// <param name="x">
        /// The matrix to multiply.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseMulInplace<T>(in this Mat<T> target, in Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            Mat.PointwiseMul(target, x, target);
        }

        /// <summary>
        /// Computes a matrix pointwise division in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be divided.
        /// </param>
        /// <param name="x">
        /// The matrix to divide.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseDivInplace<T>(in this Mat<T> target, in Mat<T> x) where T : unmanaged, INumberBase<T>
        {
            Mat.PointwiseDiv(target, x, target);
        }

        /// <summary>
        /// Transposes a matrix in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void TransposeInplace<T>(in this Mat<T> target) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfNonSquare(target, nameof(target));

            var ft = target.GetUnsafeFastIndexer();
            for (var col = 1; col < target.ColCount; col++)
            {
                for (var row = 0; row < col; row++)
                {
                    (ft[row, col], ft[col, row]) = (ft[col, row], ft[row, col]);
                }
            }
        }

        /// <summary>
        /// Applies a function to each value of the matrix in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the matrix.
        /// </typeparam>
        /// <param name="target">
        /// The target matrix to be processed.
        /// </param>
        /// <param name="func">
        /// The function to be applied.
        /// </param>
        public static void MapInplace<T>(in this Mat<T> target, Func<T, T> func) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));

            Mat.Map(target, func, target);
        }

        /// <summary>
        /// Conjugates a complex matrix in-place.
        /// </summary>
        /// <param name="target">
        /// The target complex matrix to be conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void ConjugateInplace(in this Mat<Complex> target)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));

            Mat.Conjugate(target, target);
        }

        /// <summary>
        /// Conjugates and transposes a complex matrix in-place.
        /// </summary>
        /// <param name="target">
        /// The target complex matrix to be conjugated and transposed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void ConjugateTransposeInplace(in this Mat<Complex> target)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfNonSquare(target, nameof(target));

            var ft = target.GetUnsafeFastIndexer();
            for (var col = 0; col < target.ColCount; col++)
            {
                for (var row = 0; row <= col; row++)
                {
                    if (row == col)
                    {
                        var tmp = ft[row, col];
                        ft[row, col] = tmp.Conjugate();
                    }
                    else
                    {
                        var tmp1 = ft[row, col];
                        var tmp2 = ft[col, row];
                        ft[row, col] = tmp2.Conjugate();
                        ft[col, row] = tmp1.Conjugate();
                    }
                }
            }
        }

        /// <summary>
        /// Computes a matrix inversion in-place.
        /// </summary>
        /// <param name="target">
        /// The target matrix to be inverted.
        /// </param>
        public static void InverseInplace(in this Mat<float> target)
        {
            Mat.Inverse(target, target);
        }

        /// <summary>
        /// Computes a matrix inversion in-place.
        /// </summary>
        /// <param name="target">
        /// The target matrix to be inverted.
        /// </param>
        public static void InverseInplace(in this Mat<double> target)
        {
            Mat.Inverse(target, target);
        }

        /// <summary>
        /// Computes a matrix inversion in-place.
        /// </summary>
        /// <param name="target">
        /// The target matrix to be inverted.
        /// </param>
        public static void InverseInplace(in this Mat<Complex> target)
        {
            Mat.Inverse(target, target);
        }
    }
}
