using System;
using System.Collections.Generic;
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

        internal static void ThrowIfEmpty<T>(IReadOnlyList<Vec<T>> xs, string name) where T : unmanaged, INumberBase<T>
        {
            if (xs.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one vector.", name);
            }
        }

        internal static void ThrowIfEmpty<T>(in Vec<T> v, string name) where T : unmanaged, INumberBase<T>
        {
            if (v.Count == 0)
            {
                throw new ArgumentException("The vector cannot be empty.", name);
            }
        }

        internal static void ThrowIfEmpty<T>(in Mat<T> m, string name) where T : unmanaged, INumberBase<T>
        {
            if (m.RowCount == 0 || m.ColCount == 0)
            {
                throw new ArgumentException("The matrix cannot be empty.", name);
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Vec<T> v1, in Vec<T> v2) where T : unmanaged, INumberBase<T>
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException($"The vectors must have the same length, but their lengths were '{v1.Count}' and '{v2.Count}'.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Vec<T> v1, in Vec<T> v2, in Vec<T> v3) where T : unmanaged, INumberBase<T>
        {
            if (v1.Count != v2.Count || v1.Count != v3.Count)
            {
                throw new ArgumentException($"The vectors must have the same length, but their lengths were '{v1.Count}', '{v2.Count}', and '{v3.Count}'.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Mat<T> m1, in Mat<T> m2) where T : unmanaged, INumberBase<T>
        {
            if (m1.RowCount != m2.RowCount || m1.ColCount != m2.ColCount)
            {
                throw new ArgumentException($"The matrices must have the same dimensions, but their dimensions were '{m1.RowCount} x {m1.ColCount}' and '{m2.RowCount} x {m2.ColCount}'.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Mat<T> m1, in Mat<T> m2, in Mat<T> m3) where T : unmanaged, INumberBase<T>
        {
            if (m1.RowCount != m2.RowCount || m1.ColCount != m2.ColCount || m1.RowCount != m3.RowCount || m1.ColCount != m3.ColCount)
            {
                throw new ArgumentException($"The matrices must have the same dimensions, but their dimensions were '{m1.RowCount} x {m1.ColCount}', '{m2.RowCount} x {m2.ColCount}', and '{m3.RowCount} x {m3.ColCount}'.");
            }
        }

        internal static void ThrowIfNonSquare<T>(in Mat<T> m, string name) where T : unmanaged, INumberBase<T>
        {
            if (m.RowCount != m.ColCount)
            {
                throw new ArgumentException($"The matrix must be square, but its dimensions were '{m.RowCount} x {m.ColCount}'.", name);
            }
        }
    }
}
