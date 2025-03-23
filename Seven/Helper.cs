namespace Seven
{
    public static class Helper
    {
        /// <summary>
        /// 一様順序交叉を行う
        /// </summary>
        /// <param name="x">親1</param>
        /// <param name="y">親2</param>
        /// <param name="randomIndices">ランダムなインデックスを得るための配列</param>
        /// <returns>子</returns>
        public static int[] OrderBasedCrossover(int[] x, int[] y, int[] randomIndices)
        {
            const int N = 32, K = 16;

            int[] fromX = [.. randomIndices[0..K].Select(i => x[i])];
            int[] result = new int[N];
            int fromP1Index = 0;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = fromX.Contains(y[i]) ? fromX[fromP1Index++] : y[i];
            }
            return result;
        }

        /// <summary>
        /// 配列の一部を切り取り、順番を保ったまま任意の箇所に挿入する
        /// </summary>
        /// <param name="x">配列</param>
        /// <param name="beginInclusive">切り取る半開区間の左端</param>
        /// <param name="endExclusive">切り取る半開区間の右端</param>
        /// <param name="insertIndex">挿入する箇所</param>
        /// <remarks>※ 破壊的メソッド</remarks>
        public static void CutAndInsert(int[] x, int beginInclusive, int endExclusive, int insertIndex)
        {
            int[] result = new int[x.Length];
            for (int i = 0; i < endExclusive - beginInclusive; ++i)
            {
                result[insertIndex + i] = x[i];
            }
            int from = 0, to = 0;
            while (true)
            {
                if (from >= x.Length || to >= result.Length) break;
                result[to] = x[from];
                ++from;
                ++to;
                if (from == insertIndex)
                {
                    to += endExclusive - beginInclusive;
                }
                else if (from == beginInclusive)
                {
                    from = endExclusive;
                }
            }
        }
    }
}
