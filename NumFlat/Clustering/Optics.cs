using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NumFlat.Clustering
{
    public static class Optics
    {
        private const double Undefined = double.NaN;

        public static void Fit<T>(IReadOnlyList<T> points, IDistance<T, T> distance, double eps, int minPoints, Span<int> destination)
        {
            // for each point p of DB do
            var allPoints = new OpticsPoint<T>[points.Count];
            for (var i = 0; i < allPoints.Length; i++)
            {
                // p.reachability-distance = UNDEFINED
                allPoints[i] = new OpticsPoint<T>(points[i]);
            }

            var orderedList = new List<OpticsPoint<T>>();

            // for each unprocessed point p of DB do
            for (var i = 0; i < allPoints.Length; i++)
            {
                var p = allPoints[i];
                if (p.IsProcessed)
                {
                    continue;
                }

                // N = getNeighbors(p, eps)
                var neighbors = GetNeighbors(allPoints, p.Point, distance, eps);

                // mark p as processed
                p.IsProcessed = true;

                // output p to the ordered list
                orderedList.Add(p);

                // if core-distance(p, eps, MinPts) != UNDEFINED then
                var coreDistance = CoreDistance(neighbors, eps, minPoints);
                if (coreDistance != Undefined)
                {
                    // Seeds = empty priority queue
                    var seeds = new PriorityQueue<OpticsPoint<T>, double>();

                    // update(N, p, Seeds, eps, MinPts)
                    Update(allPoints, neighbors, p, coreDistance, seeds, eps, minPoints);
                }
            }
        }

        private static IReadOnlyList<PointDistancePair<T>> GetNeighbors<T>(OpticsPoint<T>[] allPoints, T center, IDistance<T, T> distance, double eps)
        {
            var list = new List<PointDistancePair<T>>();
            foreach (var p in allPoints)
            {
                var d = distance.GetDistance(p.Point, center);
                if (d <= eps)
                {
                    list.Add(new PointDistancePair<T>(p, d));
                }
            }
            return list;
        }

        private static double CoreDistance<T>(IReadOnlyList<PointDistancePair<T>> neighbors, double eps, int minPoints)
        {
            if (neighbors.Count < minPoints)
            {
                return Undefined;
            }

            return neighbors.OrderBy(p => p.Distance).Take(minPoints).Last().Distance;
        }

        private static void Update<T>(OpticsPoint<T>[] allPoints, IReadOnlyList<PointDistancePair<T>> neighbors, OpticsPoint<T> p, double coreDistance, PriorityQueue<OpticsPoint<T>, double> seeds, double eps, int minPoints)
        {
            // for each o in N
            foreach(var o in neighbors)
            {
                // if o is not processed then
                if (o.Point.IsProcessed)
                {
                    continue;
                }

                // new-reach-dist = max(coredist, dist(p,o))
                var newReachabilityDistance = Math.Max(coreDistance, o.Distance);

                // if o.reachability-distance == UNDEFINED then // o is not in Seeds
                if (o.Point.ReachabilityDistance == Undefined)
                {
                    // o.reachability-distance = new-reach-dist
                    o.Point.ReachabilityDistance = newReachabilityDistance;

                    // Seeds.insert(o, new-reach-dist)
                    seeds.Enqueue(o.Point, newReachabilityDistance);
                }
                else // o in Seeds, check for improvement
                {
                    // if new-reach-dist < o.reachability-distance then
                    if (newReachabilityDistance < o.Point.ReachabilityDistance)
                    {
                        // o.reachability-distance = new-reach-dist
                        o.Point.ReachabilityDistance = newReachabilityDistance;

                        // Seeds.move-up(o, new-reach-dist)
                        seeds.up
                    }
                }
            }

            /*
            for (int i = 0; i < neighbors.Count; i++)
            {
                UInt32 p2Index = neighbors[i].To;

                if (_points[p2Index].WasProcessed)
                    continue;

                double newReachabilityDistance = Math.Max(coreDistance, neighbors[i].Distance);

                if (double.IsNaN(_points[p2Index].ReachabilityDistance))
                {
                    _points[p2Index].ReachabilityDistance = newReachabilityDistance;
                    _seeds.Enqueue(_points[p2Index], newReachabilityDistance);
                }
                else if (newReachabilityDistance < _points[p2Index].ReachabilityDistance)
                {
                    _points[p2Index].ReachabilityDistance = newReachabilityDistance;
                    _seeds.UpdatePriority(_points[p2Index], newReachabilityDistance);
                }
            }
            */
        }



        private class OpticsPoint<T>
        {
            public readonly T Point;
            public double ReachabilityDistance;
            public bool IsProcessed;

            public OpticsPoint(T point)
            {
                Point = point;
                ReachabilityDistance = Undefined;
                IsProcessed = false;
            }
        }

        private struct PointDistancePair<T>
        {
            public readonly OpticsPoint<T> Point;
            public readonly double Distance;

            public PointDistancePair(OpticsPoint<T> point, double distance)
            {
                Point = point;
                Distance = distance;
            }
        }
    }
}
