using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.Clustering;

namespace NumFlatTest.ClusteringTests
{
    public class KMedoidsTests
    {
        [TestCase(2)]
        public void Iris(int classCount)
        {
            var dataset = ReadIris("iris.csv").ToArray();

            Func<int, int, double> func = (i, j) => dataset[i].Item1.Distance(dataset[j].Item1);

            var medoids = KMedoids.Run(dataset.Length, classCount, func);

            foreach (var medoid in medoids)
            {
                Console.WriteLine(dataset[medoid].Item1);
            }

            Console.WriteLine();
        }

        private static IEnumerable<(Vec<double>, int)> ReadIris(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var split = line.Split(',');
                var values = split.Take(4).Select(double.Parse);
                var label = split[4] switch
                {
                    "setosa" => 0,
                    "versicolor" => 1,
                    "virginica" => 2,
                    _ => throw new Exception()
                };
                yield return (values.ToVector(), label);
            }
        }
    }
}
