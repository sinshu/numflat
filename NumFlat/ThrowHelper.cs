using System;
using System.Numerics;

namespace NumFlat
{
    internal static class ThrowHelper
    {
        internal static void ThrowIfNull<T>(T obj, string name) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        internal static void ThrowIfEmpty<T>(ref Vec<T> v, string name) where T : unmanaged, INumberBase<T>
        {
            if (v.Count == 0)
            {
                throw new ArgumentException("The vector must not be empty.", name);
            }
        }

        internal static void ThrowIfEmpty<T>(ref Mat<T> m, string name) where T : unmanaged, INumberBase<T>
        {
            if (m.RowCount == 0 || m.ColCount == 0)
            {
                throw new ArgumentException("The matrix must not be empty.", name);
            }
        }

        internal static void ThrowIfDifferentSize<T>(ref Vec<T> v1, ref Vec<T> v2) where T : unmanaged, INumberBase<T>
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(ref Vec<T> v1, ref Vec<T> v2, ref Vec<T> v3) where T : unmanaged, INumberBase<T>
        {
            if (v1.Count != v2.Count || v1.Count != v3.Count)
            {
                throw new ArgumentException("The vectors must be the same length.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(ref Mat<T> m1, ref Mat<T> m2) where T : unmanaged, INumberBase<T>
        {
            if (m1.RowCount != m2.RowCount || m1.ColCount != m2.ColCount)
            {
                throw new ArgumentException("The matrices must be the same dimensions.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(ref Mat<T> m1, ref Mat<T> m2, ref Mat<T> m3) where T : unmanaged, INumberBase<T>
        {
            if (m1.RowCount != m2.RowCount || m1.ColCount != m2.ColCount || m1.RowCount != m3.RowCount || m1.ColCount != m3.ColCount)
            {
                throw new ArgumentException("The matrices must be the same dimensions.");
            }
        }
    }
}
