using System;
using System.Collections.Generic;
using System.Linq;

namespace NumFlat.MultivariateAnalyses
{
    public class LinearDiscriminantAnalysis : IVectorToVectorTransform<double>
    {
        private Vec<double> mean;
        private GeneralizedEigenValueDecompositionDouble gevd;

        public LinearDiscriminantAnalysis(IEnumerable<Vec<double>> xs, IEnumerable<int> ys)
        {
            var mean = xs.Mean();

            var centered = xs.Select(x => x - mean);
            var grouped = centered.Zip(ys).GroupBy(pair => pair.Second).ToArray();
            var meanAndCovarianceList = grouped.Select(g => g.Select(pair => pair.First).MeanAndCovariance()).ToArray();

            var sb = meanAndCovarianceList.Select(mac => mac.Mean).Covariance();
            var sw = meanAndCovarianceList.Select(mac => mac.Covariance).Mean();

            var gevd = sb.Gevd(sw);

            this.mean = mean;
            this.gevd = gevd;
        }

        public void Transform(in Vec<double> source, in Vec<double> destination)
        {
            var tmp = source - mean;
            Mat.Mul(gevd.V, tmp, destination, true);
            destination.ReverseInplace();
        }

        /// <inheritdoc/>
        public int SourceVectorLength => mean.Count;

        /// <inheritdoc/>
        public int DestinationVectorLength => mean.Count;
    }
}
