using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.Clustering
{
    public sealed class KMedoids<T>
    {
        private readonly IDistance<T, T> distance;
        private readonly Medoid[] medoids;

        internal KMedoids(IDistance<T, T> distance, Medoid[] medoids)
        {
            this.distance = distance;
            this.medoids = medoids;
        }

        public (KMedoids<T> Model, double Error) UpdateSwap(IReadOnlyList<T> items)
        {
            // foreach xo do compute nearest(o), dnearest(o), dsecond(o);
            var nearest = Enumerable.Repeat(-1, items.Count).ToArray();
            var dNearest = Enumerable.Repeat(double.PositiveInfinity, items.Count).ToArray();
            var dSecond = Enumerable.Repeat(double.PositiveInfinity, items.Count).ToArray();
            foreach (var xo in KMedoids.EnumerateItems(items))
            {
                foreach (var medoid in medoids)
                {
                    var tmp = distance.GetDistance(xo.Item, medoid.Item);
                    if (tmp < dNearest[xo.Index])
                    {
                        dSecond[xo.Index] = dNearest[xo.Index];
                        dNearest[xo.Index] = tmp;
                        nearest[xo.Index] = medoid.Index;
                    }
                    else if (tmp < dSecond[xo.Index])
                    {
                        dSecond[xo.Index] = tmp;
                    }
                }
            }

            // repeat
            while (true)
            {
                // (dTD*, m*, x*) <- (0, null, null);
                var dTdBest = 0.0;
                var mBest = -1;
                var xBest = -1;

                // each medoid and
                // foreach mi = { m1, ..., mk } do
                foreach (var m in medoids)
                {
                    // each non-medoid
                    // foreach xc != { m1, ..., mk } do
                    foreach (var xc in KMedoids.EnumerateItemsExcept(items, this))
                    {
                        // dTD <- 0;
                        var dTd = 0.0;

                        // foreach xo != { m1, ..., mk } \ mi do
                        foreach(var xo
                    }
                }
            }
        }

        public IDistance<T, T> Distance => distance;
        public IReadOnlyList<Medoid> Medoids => medoids;



        public class Medoid
        {
            private int index;
            private T item;

            internal Medoid(int index, T item)
            {
                this.index = index;
                this.item = item;
            }

            public int Index => index;
            public T Item => item;
        }
    }

    public sealed class KMedoids
    {
        // This implementation is based on the following thesis:
        // https://www.sciencedirect.com/science/article/pii/S0306437921000557

        public static (KMedoids<T> Model, double Error) GetInitialGuessBuild<T>(IReadOnlyList<T> items, IDistance<T, T> distance, int clusterCount)
        {
            // (TD, m1) <- (oo, null);
            var td = double.PositiveInfinity;
            var m0 = -1;

            // choose the first medoid
            // foreach xc do
            foreach (var xc in EnumerateItems(items))
            {
                // TDj <- 0;
                var tdj = 0.0;

                // foreach xo != xc do TDj <- TDj + d(xo, xc);
                foreach (var xo in EnumerateItemsExcept(items, xc.Index))
                {
                    tdj += distance.GetDistance(xo.Item, xc.Item);
                }

                // smallest distance sum
                // if TDj < TD then
                if (tdj < td)
                {
                    // (TD, m1) <- (TDj, xc)
                    td = tdj;
                    m0 = xc.Index;
                }
            }

            // initialize distance to nearest medoid
            // foreach xo != m1 do dnearest(o) <- d(m1, xo);
            var dNearest = new double[items.Count];
            foreach (var xo in EnumerateItemsExcept(items, m0))
            {
                dNearest[xo.Index] = distance.GetDistance(items[m0], xo.Item);
            }

            // choose remaining medoids
            // for i = 1...k-1 do
            List<int> ms = [m0];
            for (var i = 0; i < clusterCount - 1; i++)
            {
                // (dTD*, x*) <- (oo, null);
                var dTdBest = double.PositiveInfinity;
                var xBest = -1;

                // foreach xc != { m1, ..., mi } do
                foreach (var xc in EnumerateItemsExcept(items, ms))
                {
                    // dTD <- 0;
                    var dTd = 0.0;

                    // foreach xo != { m1, ..., mi, xc } do
                    foreach (var xo in EnumerateItemsExcept(items, ms, xc.Index))
                    {
                        // reduction in TD
                        // delta <- d(xo, xc) - dnearest(o);
                        var delta = distance.GetDistance(xo.Item, xc.Item) - dNearest[xo.Index];

                        // if delta < 0 then dTD <- dTD + delta;
                        if (delta < 0.0)
                        {
                            dTd += delta;
                        }
                    }

                    // best reduction
                    // if dTD < dTD* then
                    if (dTd < dTdBest)
                    {
                        // (dTD*, x*) <- (dTD, xc)
                        dTdBest = dTd;
                        xBest = xc.Index;
                    }
                }

                // (TD, mi+1) <- (TD + dTD*, x*);
                td += dTdBest;
                ms.Add(xBest);

                // update distances to nearest medoid
                // foreach xo != { m1, ..., mi+1, } do
                var mLast = ms.Last();
                foreach (var xo in EnumerateItemsExcept(items, ms))
                {
                    // dnearest(o) <- min { dnearest(o), d(xo, mi+1) };
                    var tmp = dNearest[xo.Index];
                    dNearest[xo.Index] = Math.Min(tmp, distance.GetDistance(xo.Item, items[mLast]));
                }
            }

            // return TD, { m1, ..., mk };
            var medoids = new KMedoids<T>.Medoid[clusterCount];
            for (var i = 0; i < medoids.Length; i++)
            {
                medoids[i] = new KMedoids<T>.Medoid(ms[i], items[ms[i]]);
            }
            return (new KMedoids<T>(distance, medoids), td);
        }

        internal static IEnumerable<(int Index, T Item)> EnumerateItems<T>(IEnumerable<T> items)
        {
            var count = 0;
            foreach (var item in items)
            {
                yield return (count, item);
                count++;
            }
        }

        internal static IEnumerable<(int Index, T Item)> EnumerateItemsExcept<T>(IEnumerable<T> items, int excludeIndex)
        {
            foreach (var tuple in EnumerateItems(items))
            {
                if (tuple.Index != excludeIndex)
                {
                    yield return tuple;
                }
            }
        }

        internal static IEnumerable<(int Index, T Item)> EnumerateItemsExcept<T>(IEnumerable<T> items, IReadOnlyList<int> excludeIndices)
        {
            foreach (var tuple in EnumerateItems(items))
            {
                if (excludeIndices.All(index => index != tuple.Index))
                {
                    yield return tuple;
                }
            }
        }

        internal static IEnumerable<(int Index, T Item)> EnumerateItemsExcept<T>(IEnumerable<T> items, IReadOnlyList<int> excludeIndices, int anotherExcludeIndex)
        {
            foreach (var tuple in EnumerateItemsExcept(items, excludeIndices))
            {
                if (tuple.Index != anotherExcludeIndex)
                {
                    yield return tuple;
                }
            }
        }

        internal static IEnumerable<(int Index, T Item)> EnumerateItemsExcept<T>(IEnumerable<T> items, KMedoids<T> medoids)
        {
            foreach (var tuple in EnumerateItems(items))
            {
                if (medoids.Medoids.All(medoid => medoid.Index != tuple.Index))
                {
                    yield return tuple;
                }
            }
        }
    }
}
