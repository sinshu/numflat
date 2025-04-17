using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumFlat
{
    internal static class EnumerableHelper
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

        internal static IEnumerable<Vec<T>> ThrowIfEmptyOrDifferentSize<T>(this IEnumerable<Vec<T>> xs, int length, string name) where T : unmanaged, INumberBase<T>
        {
            foreach (var x in xs)
            {
                if (x.Count == 0)
                {
                    throw new ArgumentException("Empty vectors are not allowed.");
                }

                if (x.Count != length)
                {
                    throw new ArgumentException($"The length of the vectors must be {length}, but a vector with a length {x.Count} was found.");
                }

                yield return x;
            }
        }

        internal static IReadOnlyList<IReadOnlyList<Vec<T>>> GroupByClassIndex<T>(IEnumerable<Vec<T>> xs, IEnumerable<int> ys) where T : unmanaged, INumberBase<T>
        {
            var dic = new Dictionary<int, List<Vec<T>>>();
            using (var exs = xs.GetEnumerator())
            using (var eys = ys.GetEnumerator())
            {
                while (true)
                {
                    var xsHasNext = exs.MoveNext();
                    var ysHasNext = eys.MoveNext();
                    if (xsHasNext != ysHasNext)
                    {
                        throw new ArgumentException("The number of source vectors and class indices must match.");
                    }

                    if (xsHasNext)
                    {
                        var x = exs.Current;
                        var y = eys.Current;

                        List<Vec<T>>? list;
                        if (!dic.TryGetValue(y, out list))
                        {
                            list = new List<Vec<T>>();
                            dic.Add(y, list);
                        }
                        list.Add(x);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (dic.Count < 2)
            {
                throw new ArgumentException("A minimum of two classes are required.");
            }

            var expected = 0;
            foreach (var actual in dic.Keys.Order())
            {
                if (actual != expected)
                {
                    throw new ArgumentException($"Class index must be a sequential integer starting from zero, but the class index '{expected}' was missing.");
                }
                expected++;
            }

            return dic.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToArray();
        }
    }
}
