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
            int[] fromX = [.. randomIndices[0 .. (x.Length / 2)].Select(i => x[i])];
            int[] result = new int[x.Length];
            int xIndex = 0;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = fromX.Contains(y[i]) ? fromX[xIndex++] : y[i];
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
        /// <returns>新たな配列</returns>
        public static int[] CutAndInsert(int[] x, int beginInclusive, int endExclusive, int insertIndex)
        {
            int[] result = new int[x.Length];
            for (int i = 0; i < endExclusive - beginInclusive; ++i)
            {
                result[insertIndex + i] = x[beginInclusive + i];
            }
            int[] tmp = [.. x[.. beginInclusive], .. x[endExclusive ..]];
            for (int i = 0; i < insertIndex; ++i)
            {
                result[i] = tmp[i];
            }
            for (int i = 0; i < tmp.Length - insertIndex; ++i)
            {
                result[^(i + 1)] = tmp[^(i + 1)];
            }
            return result;
        }
    }
}
