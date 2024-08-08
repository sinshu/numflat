using System;
using System.Buffers;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NumFlat.Clustering
{
    /// <summary>
    /// Provides the DBSCAN (density-based spatial clustering of applications with noise) clustering algorithm.
    /// </summary>
    public static class DbScan
    {
        /// <summary>
        /// Applies the DBSCAN (density-based spatial clustering of applications with noise) algorithm 
        /// to the given set of points.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the points.
        /// </typeparam>
        /// <param name="points">
        /// The collection of points to be clustered.
        /// </param>
        /// <param name="distance">
        /// The distance metric to calculate the distance between points.
        /// </param>
        /// <param name="eps">
        /// The maximum distance between two points for them to be considered as in the same neighborhood.
        /// </param>
        /// <param name="minPoints">
        /// The minimum number of points required to form a cluster.
        /// </param>
        /// <param name="destination">
        /// The destination where the cluster labels for each point will be stored. 
        /// A label of -1 indicates noise, and any non-negative integer value indicates the cluster to which the point belongs.
        /// </param>
        public static void Fit<T>(IReadOnlyList<T> points, IDistance<T, T> distance, double eps, int minPoints, Span<int> destination)
        {
            ThrowHelper.ThrowIfNull(points, nameof(points));
            ThrowHelper.ThrowIfNull(distance, nameof(distance));

            if (points.Count == 0)
            {
                throw new ArgumentException("The sequence must contain at least one point.", nameof(points));
            }

            if (eps <= 0)
            {
                throw new ArgumentException("The eps must be a non-negative value.", nameof(eps));
            }

            if (minPoints <= 0)
            {
                throw new ArgumentException("The minimum number of points must be a non-negative value.", nameof(minPoints));
            }

            if (destination.Length != points.Count)
            {
                throw new ArgumentException("The length of destination must match the number of points.", nameof(destination));
            }

            var allPoints = new DbScanPoint<T>[points.Count];
            for (var i = 0; i < allPoints.Length; i++)
            {
                allPoints[i] = new DbScanPoint<T>(points[i]);
            }

            int clusterIndex = 0;
            for (var i = 0; i < allPoints.Length; i++)
            {
                var p = allPoints[i];
                if (p.IsVisited)
                {
                    continue;
                }

                p.IsVisited = true;

                var neighborPoints = RegionQuery(allPoints, p.Point, distance, eps);
                if (neighborPoints.Length < minPoints)
                {
                    p.ClusterIndex = -1;
                }
                else
                {
                    clusterIndex++;
                    ExpandCluster(allPoints, p, neighborPoints, clusterIndex, distance, eps, minPoints);
                }
            }

            for (var i = 0; i < allPoints.Length; i++)
            {
                var index = allPoints[i].ClusterIndex;
                destination[i] = index != -1 ? index - 1 : -1;
            }
        }

        private static void ExpandCluster<T>(DbScanPoint<T>[] allPoints, DbScanPoint<T> center, DbScanPoint<T>[] neighborPoints, int clusterIndex, IDistance<T, T> distance, double eps, int minPoints)
        {
            center.ClusterIndex = clusterIndex;

            var queue = new Queue<DbScanPoint<T>>(neighborPoints);
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                if (p.ClusterIndex == 0)
                {
                    p.ClusterIndex = clusterIndex;
                }

                if (p.IsVisited)
                {
                    continue;
                }

                p.IsVisited = true;

                var neighborPoints2 = RegionQuery(allPoints, p.Point, distance, eps);
                if (neighborPoints2.Length >= minPoints)
                {
                    foreach (var p2 in neighborPoints2.Where(neighbor => !neighbor.IsVisited))
                    {
                        queue.Enqueue(p2);
                    }
                }
            }
        }

        private static DbScanPoint<T>[] RegionQuery<T>(DbScanPoint<T>[] allPoints, T center, IDistance<T, T> distance, double eps)
        {
            return allPoints.Where(point => distance.GetDistance(point.Point, center) <= eps).ToArray();
        }



        private class DbScanPoint<T>
        {
            public readonly T Point;
            public bool IsVisited;
            public int ClusterIndex;

            public DbScanPoint(T point)
            {
                Point = point;
                IsVisited = false;
                ClusterIndex = 0;
            }
        }
    }
}
