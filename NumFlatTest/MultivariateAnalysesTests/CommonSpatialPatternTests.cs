using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using NUnit.Framework;
using NumFlat;
using NumFlat.MultivariateAnalyses;

namespace NumFlatTest.MultivariateAnalysesTests
{
    public class CommonSpatialPatternTests
    {
        [Test]
        public void Test()
        {
            var input = ReadData("csp_input.csv").ToArray();
            var xs = input.Select(tpl => tpl.Item1).ToArray();
            var ys = input.Select(tpl => tpl.Item2).ToArray();

            var output = ReadData("csp_output.csv").ToArray();
            var rawExpectedList = output.Select(tpl => tpl.Item1).ToArray();

            var csp = xs.Csp(ys);
            var actualList = xs.Select(v => csp.Transform(v)).ToArray();

            var scale = actualList[0].PointwiseDiv(rawExpectedList[0]);

            foreach (var (actual, rawExpected) in actualList.Zip(rawExpectedList))
            {
                var expected = rawExpected.PointwiseMul(scale);
                NumAssert.AreSame(expected, actual, 1e-12);
            }
        }

        private static IEnumerable<(Vec<double>, int)> ReadData(string filename)
        {
            var path = Path.Combine("dataset", filename);
            foreach (var line in File.ReadLines(path).Skip(1))
            {
                var split = line.Split(',');
                var values = split.Take(3).Select(double.Parse);
                var label = split[3] switch
                {
                    "0.0" => 0,
                    "1.0" => 1,
                    _ => -1
                };

                if (label != -1)
                {
                    yield return (values.ToVector(), label);
                }
            }
        }
    }
}
