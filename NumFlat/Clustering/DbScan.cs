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
            //
            // This is based on the following implementations.
            // https://ja.wikipedia.org/wiki/DBSCAN
            // https://github.com/yusufuzun/dbscan
            // https://github.com/acse-hy23/DBSCAN-with-KDTree
            //

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

            // C = 0
            int c = 0;

            // for each point P in dataset D {
            foreach (var p in allPoints)
            {
                // if P is visited
                if (p.IsVisited)
                {
                    // continue next point
                    continue;
                }

                // mark P as visited
                p.IsVisited = true;

                // NeighborPts = regionQuery(P, eps)
                var neighborPoints = RegionQuery(allPoints, p.Point, distance, eps);

                // if sizeof(NeighborPts) < MinPts
                if (neighborPoints.Length < minPoints)
                {
                    // mark P as NOISE
                    p.ClusterIndex = -1;
                }
                else
                {
                    // C = next cluster
                    c++;

                    // expandCluster(P, NeighborPts, C, eps, MinPts)
                    ExpandCluster(allPoints, p, neighborPoints, c, distance, eps, minPoints);
                }
            }

            for (var i = 0; i < allPoints.Length; i++)
            {
                var index = allPoints[i].ClusterIndex;
                destination[i] = index != -1 ? index - 1 : -1;
            }
        }

        private static void ExpandCluster<T>(DbScanPoint<T>[] allPoints, DbScanPoint<T> center, DbScanPoint<T>[] neighborPoints, int c, IDistance<T, T> distance, double eps, int minPoints)
        {
            // add P to cluster C
            center.ClusterIndex = c;

            // for each point P' in NeighborPts { 
            var queue = new Queue<DbScanPoint<T>>(neighborPoints);
            while (queue.Count > 0)
            {
                var p = queue.Dequeue();

                // if P' is not visited {
                if (!p.IsVisited)
                {
                    // mark P' as visited
                    p.IsVisited = true;

                    // NeighborPts' = regionQuery(P', eps)
                    var neighborPoints2 = RegionQuery(allPoints, p.Point, distance, eps);

                    // if sizeof(NeighborPts') >= MinPts
                    if (neighborPoints2.Length >= minPoints)
                    {
                        // NeighborPts = NeighborPts joined with NeighborPts'
                        var newPoints = neighborPoints2.Except(neighborPoints);
                        foreach (var p2 in newPoints)
                        {
                            queue.Enqueue(p2);
                        }
                    }
                }

                // if P' is not yet member of any cluster
                if (p.ClusterIndex == 0)
                {
                    // add P' to cluster C
                    p.ClusterIndex = c;
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
