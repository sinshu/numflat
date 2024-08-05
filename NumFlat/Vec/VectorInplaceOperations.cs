using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides in-place operations for <see cref="Vec{T}"/>.
    /// </summary>
    public static class VectorInplaceOperations
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
        /// Computes a pointwise vector-and-scalar addition in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be added.
        /// </param>
        /// <param name="x">
        /// The scalar to add.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void AddInplace<T>(in this Vec<T> target, T x) where T : unmanaged, INumberBase<T>
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
        /// Computes a pointwise vector-and-scalar subtraction in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be subtracted.
        /// </param>
        /// <param name="x">
        /// The scalar to sbtract.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void SubInplace<T>(in this Vec<T> target, T x) where T : unmanaged, INumberBase<T>
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
        /// Reverses the order of elements in a vector in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be reversed.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void ReverseInplace<T>(in this Vec<T> target) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));

            var span = target.Memory.Span;
            var p1 = 0;
            var p2 = span.Length - 1;
            while(p1 < p2)
            {
                (span[p1], span[p2]) = (span[p2], span[p1]);
                p1 += target.Stride;
                p2 -= target.Stride;
            }
        }

        /// <summary>
        /// Applies a function to each value of the vector in-place.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be processed.
        /// </param>
        /// <param name="func">
        /// The function to be applied.
        /// </param>
        public static void MapInplace<T>(in this Vec<T> target, Func<T, T> func) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));

            Vec.Map(target, func, target);
        }

        /// <summary>
        /// Conjugates the complex vector in-place.
        /// </summary>
        /// <param name="target">
        /// The target complex vector to be conjugated.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void ConjugateInplace(in this Vec<Complex> target)
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));

            Vec.Conjugate(target, target);
        }

        /// <summary>
        /// Gets values from an <see cref="IEnumerable{T}"/> and sets the values to the vector.
        /// </summary>
        /// <typeparam name="T">
        /// The type of elements in the vector.
        /// </typeparam>
        /// <param name="target">
        /// The target vector to be set.
        /// </param>
        /// <param name="source">
        /// The source enumerable to be read.
        /// </param>
        public static void SetInplace<T>(in this Vec<T> target, IEnumerable<T> source) where T : unmanaged, INumberBase<T>
        {
            ThrowHelper.ThrowIfEmpty(target, nameof(target));
            ThrowHelper.ThrowIfNull(source, nameof(source));

            var e = source.GetEnumerator();
            foreach (ref var value in target)
            {
                if (!e.MoveNext())
                {
                    throw new ArgumentException("The lengths of the target vector and the source list must match.");
                }

                value = e.Current;
            }

            if (e.MoveNext())
            {
                throw new ArgumentException("The lengths of the target vector and the source list must match.");
            }
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="target">
        /// The target vector to be normalized.
        /// </param>
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
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
        /// <remarks>
        /// This method does not allocate managed heap memory.
        /// </remarks>
        public static void NormalizeInplace(in this Vec<Complex> target, double p)
        {
            Vec.Noramlize(target, target, p);
        }
    }
}
