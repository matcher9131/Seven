using Seven.Core;

namespace Seven.GA
{
    public class Crossover(IRandom random)
    {
        private const int N = 32;
        private const int K = 16;
        private readonly int[] indices = [.. Enumerable.Range(0, N)];

        private readonly IRandom random = random;

        public void RandomizeIndices()
        {
            // 全体をランダムにシャッフルしてからOrderBasedCrossoverのために先頭K個を昇順にソートする
            for (int i = this.indices.Length - 1; i > 0; --i)
            {
                int j = random.Next(i + 1);
                (this.indices[i], this.indices[j]) = (this.indices[j], this.indices[i]);
            }
            Array.Sort(this.indices, 0, K);
        }

        /// <summary>
        /// 一様順序交叉を行う
        /// </summary>
        /// <param name="x">親1</param>
        /// <param name="y">親2</param>
        /// <returns>子</returns>
        public int[] Cross(int[] x, int[] y)
        {
            int[] fromX = new int[indices.Length];
            for (int i = 0; i < fromX.Length; ++i)
            {
                fromX[i] = x[indices[i]];
            }
            int[] result = new int[x.Length];
            int xIndex = 0;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = fromX.Contains(y[i]) ? fromX[xIndex++] : y[i];
            }
            return result;
        }
    }
}
