using System;
using System.Collections.Generic;

namespace NumFlat.TimeSeries
{
    public static class DtwBarycenterAveraging
    {
        public static IReadOnlyList<Vec<double>> Fit(IReadOnlyList<IReadOnlyList<Vec<double>>> sequences, DtwBarycenterAveragingOptions options)
        {
            // DTW barycenter averagingを実行して、平均シーケンスを返す
            // optionのmaxIterationsとtoleranceを使用して収束判定を行う
            return null;
        }

        public static IReadOnlyList<Vec<double>> GetInitialGuess(IReadOnlyList<IReadOnlyList<Vec<double>>> sequences)
        {
            // DTW距離が最小となるシーケンスを初期推定値として選択する
            return null;
        }

        public static (Vec<double>[] Sequence, double Error) Update(IReadOnlyList<IReadOnlyList<Vec<double>>> sequences)
        {
            // DTW barycenter averagingのイテレーション一回分を実行
            return (null, 0.0);
        }
    }
}
