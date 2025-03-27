using Seven.Core.Random;
using System.Collections.Immutable;

namespace Seven.GA
{
    public class Crossover(IRandom random)
    {
        private const int K = 16;
        private static readonly int[] Indices = [.. Enumerable.Range(0, Graph.NumVertexes)];

        private readonly IRandom random = random;

        private List<int> GetRandomIndices()
        {
            // 昇順を保つため、くじを戻さないくじ引き方式でインデックスの昇順に採用/不採用を決める
            List<int> result = new(K);
            for (int i = 0; i < Indices.Length; ++i)
            {
                if (this.random.Next((uint)(Indices.Length - i)) < K - result.Count)
                {
                    result.Add(Indices[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 一様順序交叉を行う
        /// </summary>
        /// <param name="p1">親1</param>
        /// <param name="p2">親2</param>
        /// <returns>子</returns>
        public (int[] c1, int[] c2) Cross(int[] p1, int[] p2)
        {
            List<int> randomIndices = this.GetRandomIndices();

            int[] crossInner(int[] x, int[] y)
            {
                Span<int> fromX = stackalloc int[K];
                for (int i = 0; i < fromX.Length; ++i)
                {
                    fromX[i] = x[randomIndices[i]];
                }
                int[] result = new int[x.Length];
                int xIndex = 0;
                for (int i = 0; i < result.Length; ++i)
                {
                    result[i] = fromX.Contains(y[i]) ? fromX[xIndex++] : y[i];
                }
                return result;
            }

            return (crossInner(p1, p2), crossInner(p2, p1));
        }
    }
}
