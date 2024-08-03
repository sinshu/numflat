using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public sealed class LogisticRegression
    {
        public LogisticRegression(IReadOnlyList<Vec<double>> xs, IEnumerable<int> ys)
        {
            var n = xs.Count;
            var d = xs[0].Count + 1;

            var trainX = new Mat<double>(n, d);
            foreach (var (x, row) in xs.Zip(trainX.Cols[0..xs[0].Count].Rows))
            {
                x.CopyTo(row);
            }
            trainX.Cols.Last().Fill(1);

            Console.WriteLine(trainX);
        }
    }
}
