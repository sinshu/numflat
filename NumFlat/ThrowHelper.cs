﻿using System;
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
                throw new ArgumentException("The sequence must contain at least one vector.", nameof(xs));
            }
        }

        internal static void ThrowIfEmpty<T>(in Vec<T> v, string name) where T : unmanaged, INumberBase<T>
        {
            if (v.Count == 0)
            {
                throw new ArgumentException("An empty vector is not allowed.", name);
            }
        }

        internal static void ThrowIfEmpty<T>(in Mat<T> m, string name) where T : unmanaged, INumberBase<T>
        {
            if (m.RowCount == 0 || m.ColCount == 0)
            {
                throw new ArgumentException("An empty matrix is not allowed.", name);
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Vec<T> v1, in Vec<T> v2) where T : unmanaged, INumberBase<T>
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException("The vectors must have the same length.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Vec<T> v1, in Vec<T> v2, in Vec<T> v3) where T : unmanaged, INumberBase<T>
        {
            if (v1.Count != v2.Count || v1.Count != v3.Count)
            {
                throw new ArgumentException("The vectors must have the same length.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Mat<T> m1, in Mat<T> m2) where T : unmanaged, INumberBase<T>
        {
            if (m1.RowCount != m2.RowCount || m1.ColCount != m2.ColCount)
            {
                throw new ArgumentException("The matrices must have the same dimensions.");
            }
        }

        internal static void ThrowIfDifferentSize<T>(in Mat<T> m1, in Mat<T> m2, in Mat<T> m3) where T : unmanaged, INumberBase<T>
        {
            if (m1.RowCount != m2.RowCount || m1.ColCount != m2.ColCount || m1.RowCount != m3.RowCount || m1.ColCount != m3.ColCount)
            {
                throw new ArgumentException("The matrices must have the same dimensions.");
            }
        }

        internal static void ThrowIfNonSquare<T>(in Mat<T> m, string name) where T : unmanaged, INumberBase<T>
        {
            if (m.RowCount != m.ColCount)
            {
                throw new ArgumentException("The matrix must be a square matrix.", name);
            }
        }
    }
}
