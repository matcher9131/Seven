﻿using Seven.Core;
using System.Collections.Immutable;

namespace Seven.GA
{
    public class Crossover(IRandom random)
    {
        private const int K = 16;

        private readonly IRandom random = random;

        private List<int> GetRandomIndices()
        {
            List<int> result = [];
            ImmutableArray<int> vertexes = Graph.GetVertexes();
            for (int i = 0; i < vertexes.Length; ++i)
            {
                if (this.random.Next(vertexes.Length) < K - result.Count)
                {
                    result.Add(vertexes[i]);
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
            List<int> indices = this.GetRandomIndices();

            int[] crossInner(int[] x, int[] y)
            {
                int[] fromX = new int[indices.Count];
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

            return (crossInner(p1, p2), crossInner(p2, p1));
        }
    }
}
