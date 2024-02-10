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
    }
}
