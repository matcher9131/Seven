using Seven.Core;
using System;

namespace Seven.GA
{
    public static class Helper
    {
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
