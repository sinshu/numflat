using System;
using System.Collections.Generic;

namespace NumFlat.TimeSeries
{
    public static class DtwBarycenterAveraging
    {
        public static IReadOnlyList<T> Fit<T>(IReadOnlyList<IReadOnlyList<T>> sequences, DistanceMetric<T, T> dm, DtwBarycenterAveragingOptions options)
        {
            // DTW barycenter averagingを実行して、平均シーケンスを返す
            // optionのmaxIterationsとtoleranceを使用して収束判定を行う
            return null;
        }

        public static IReadOnlyList<T> GetInitialGuess<T>(IReadOnlyList<IReadOnlyList<T>> sequences, DistanceMetric<T, T> dm)
        {
            // DTW距離が最小となるシーケンスを初期推定値として選択する
            return null;
        }

        public static (T[] Sequence, double Error) Update<T>(IReadOnlyList<IReadOnlyList<T>> sequences, DistanceMetric<T, T> dm)
        {
            // DTW barycenter averagingのイテレーション一回分を実行
            return (null, 0.0);
        }
    }
}
