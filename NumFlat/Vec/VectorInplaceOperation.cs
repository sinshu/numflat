using System;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides in-place operations for <see cref="Vec{T}"/>.
    /// </summary>
    public static class VectorInplaceOperation
    {
        /// <summary>
        /// Computes a vector addition in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be added.
        /// </param>
        /// <param name="x">
        /// The vector to add.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void AddInplace<T>(in this Vec<T> target, in Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            Vec.Add(target, x, target);
        }

        /// <summary>
        /// Computes a vector subtraction in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be subtracted.
        /// </param>
        /// <param name="x">
        /// The vector to sbtract.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void SubInplace<T>(in this Vec<T> target, in Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            Vec.Sub(target, x, target);
        }

        /// <summary>
        /// Computes a vector-and-scalar multiplication in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be multiplied.
        /// </param>
        /// <param name="x">
        /// The scalar to multiply.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void MulInplace<T>(in this Vec<T> target, T x) where T : unmanaged, INumberBase<T>
        {
            Vec.Mul(target, x, target);
        }

        /// <summary>
        /// Computes a vector-and-scalar division in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be divided.
        /// </param>
        /// <param name="x">
        /// The scalar to divide.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void DivInplace<T>(in this Vec<T> target, T x) where T : unmanaged, INumberBase<T>
        {
            Vec.Div(target, x, target);
        }

        /// <summary>
        /// Computes a vector pointwise multiplication in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be multiplied.
        /// </param>
        /// <param name="x">
        /// The vector to multiply.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseMulInplace<T>(in this Vec<T> target, in Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            Vec.PointwiseMul(target, x, target);
        }

        /// <summary>
        /// Computes a vector pointwise division in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be divided.
        /// </param>
        /// <param name="x">
        /// The vector to divide.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void PointwiseDivInplace<T>(in this Vec<T> target, in Vec<T> x) where T : unmanaged, INumberBase<T>
        {
            Vec.PointwiseDiv(target, x, target);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        public static void NormalizeInplace(in this Vec<float> target)
        {
            Vec.Noramlize(target, target);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        public static void NormalizeInplace(in this Vec<float> target, float p)
        {
            Vec.Noramlize(target, target, p);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        public static void NormalizeInplace(in this Vec<double> target)
        {
            Vec.Noramlize(target, target);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        public static void NormalizeInplace(in this Vec<double> target, double p)
        {
            Vec.Noramlize(target, target, p);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        public static void NormalizeInplace(in this Vec<Complex> target)
        {
            Vec.Noramlize(target, target);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        /// <param name="p">
        /// The p value.
        /// </param>
        public static void NormalizeInplace(in this Vec<Complex> target, double p)
        {
            Vec.Noramlize(target, target, p);
        }
    }
}
