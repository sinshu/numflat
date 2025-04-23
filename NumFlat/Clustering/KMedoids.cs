using System;
using System.Linq;


namespace NumFlat.Clustering
{
    public static class KMedoids
    {
        /// <summary>
        /// k-medoids を FasterPAM (Algorithm 4) で求める
        /// </summary>
        /// <param name="dist">距離行列 d(i,j) を返すデリゲート</param>
        public static int[] Run(int n, int k, Func<int, int, double> dist,
                                int maxIter = int.MaxValue)
        {
            // ---------- BUILD フェーズ（元の Algorithm 1、割愛してランダム初期化） ----------
            var medoids = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid())
                                    .Take(k).ToArray();

            // 近接情報をキャッシュ
            var nearest = new int[n];          // nearest(o)      – 行 2
            var dNear = new double[n];       // d_nearest(o)
            var dSecond = new double[n];       // d_second(o)
            UpdateCaches();

            // ---- Algorithm 4 1行目相当 ----
            int xLast = -1;                    // xlast ← invalid

            // ---- 外側 repeat ループ（行 4 〜） ----
            for (int iter = 0; iter < maxIter; iter++)
            {
                // 行 3: ΔTD−m₁ … ΔTD−m_k を計算
                double[] deltaRemove = ComputeRemovalLoss();

                bool anyImproved = false;

                // 行 5: すべての non-medoid x_c を走査
                foreach (int xc in Enumerable.Range(0, n).Except(medoids))
                {
                    // 行 6: 改善が無ければ break
                    if (xc == xLast) { anyImproved = false; break; }

                    // --------- 行 7 & 8 ----------
                    var deltaTd = (double[])deltaRemove.Clone(); // ΔTD ← ...
                    double shared = 0.0;                         // ΔTD+_xc

                    // --------- 行 9〜15 ----------
                    for (int o = 0; o < n; o++)
                    {
                        double dOj = dist(o, xc); // 行 10

                        if (dOj < dNear[o])       // 行 11  case(i)
                        {
                            shared += dOj - dNear[o];
                            int iMed = nearest[o];
                            deltaTd[iMed] += dNear[o] - dSecond[o]; // 行 13
                        }
                        else if (dOj < dSecond[o]) // 行 14  case(ii/iii)
                        {
                            int iMed = nearest[o];
                            deltaTd[iMed] += dOj - dSecond[o];
                        }
                    }

                    // --------- 行 16–18 ----------
                    int bestIdx = ArgMin(deltaTd);       // argmin ΔTDᵢ
                    double bestGain = deltaTd[bestIdx] + shared;

                    if (bestGain < 0) // 行 18: eager swap
                    {
                        // 行 19: swap
                        int mStar = medoids[bestIdx];
                        medoids[bestIdx] = xc;

                        // キャッシュ更新（行 21）
                        UpdateCaches();

                        // ΔTD−mⱼ の再計算（行 21）
                        deltaRemove = ComputeRemovalLoss();

                        // 行 22: 記録
                        xLast = xc;
                        anyImproved = true;
                    }
                }

                if (!anyImproved) break; // 1パス改善なし → 収束
            }

            return medoids;

            // ======  内部ヘルパ ======
            void UpdateCaches()
            {
                for (int o = 0; o < n; o++)
                {
                    double best = double.PositiveInfinity, second = double.PositiveInfinity;
                    int bestIdx = -1;

                    for (int idx = 0; idx < k; idx++)
                    {
                        int m = medoids[idx];
                        double d = (o == m) ? 0 : dist(o, m);
                        if (d < best)
                        {
                            second = best; best = d; bestIdx = idx;
                        }
                        else if (d < second)
                        {
                            second = d;
                        }
                    }
                    nearest[o] = bestIdx;
                    dNear[o] = best;
                    dSecond[o] = second;
                }
            }

            double[] ComputeRemovalLoss() // ΔTD−mⱼ  … 式 (9)
            {
                var loss = new double[k];
                for (int o = 0; o < n; o++)
                {
                    int iMed = nearest[o];
                    loss[iMed] += dSecond[o] - dNear[o];
                }
                return loss;
            }

            static int ArgMin(double[] a)
            {
                double best = double.PositiveInfinity;
                int idx = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] < best) { best = a[i]; idx = i; }
                }
                return idx;
            }
        }
    }
}
