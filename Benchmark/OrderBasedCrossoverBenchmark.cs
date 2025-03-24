using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class OrderBasedCrossoverBenchmark
    {
        private static readonly int[] x = [12, 5, 27, 1, 30, 29, 4, 19, 16, 18, 31, 14, 13, 20, 6, 3, 17, 26, 22, 8, 15, 23, 10, 11, 28, 24, 21, 25, 0, 7, 9, 2];
        private static readonly int[] y = [26, 29, 3, 24, 13, 30, 11, 8, 17, 12, 16, 31, 20, 19, 4, 2, 7, 28, 14, 6, 23, 15, 25, 22, 18, 21, 1, 10, 5, 27, 9, 0];
        private static readonly int[] indices = [2, 20, 7, 14, 21, 4, 0, 12, 29, 28, 31, 1, 18, 3, 11, 10, 25, 27, 30, 23, 26, 8, 9, 24, 5, 6, 16, 13, 19, 15, 22, 17];

        private static int[] OrderBasedCrossoverWithSelect(int[] x, int[] y, int[] indices)
        {
            int[] fromX = [.. indices[0..(x.Length / 2)].Select(i => x[i])];
            int[] result = new int[x.Length];
            int xIndex = 0;
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = fromX.Contains(y[i]) ? fromX[xIndex++] : y[i];
            }
            return result;
        }

        private static int[] OrderBasedCrossoverWithoutSelect(int[] x, int[] y, ReadOnlySpan<int> indices)
        {
            int[] fromX = new int[x.Length / 2];
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

        [Benchmark]
        public int[] WithSelect()
        {
            return OrderBasedCrossoverWithSelect(x, y, indices);
        }

        [Benchmark]
        public int[] WithoutSelect()
        {
            return OrderBasedCrossoverWithoutSelect(x, y, indices.AsSpan()[..(indices.Length / 2)]);
        }
    }
}
