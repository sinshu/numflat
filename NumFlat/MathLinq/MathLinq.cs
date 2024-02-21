using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumFlat
{
    /// <summary>
    /// Provides LINQ-like operators for matrices and vectors.
    /// </summary>
    public static partial class MathLinq
    {
        internal static IEnumerable<Vec<T>> ThrowIfEmptyOrDifferentSize<T>(this IEnumerable<Vec<T>> xs, string name) where T : unmanaged, INumberBase<T>
        {
            var length = 0;
            foreach (var x in xs)
            {
                if (x.Count == 0)
                {
                    throw new ArgumentException("Empty vectors are not allowed.");
                }

                if (length == 0)
                {
                    length = x.Count;
                }

                if (x.Count != length)
                {
                    throw new ArgumentException("All the vectors must have the same length.");
                }

                yield return x;
            }
        }
    }
}
