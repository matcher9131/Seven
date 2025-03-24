using BenchmarkDotNet.Attributes;

namespace Benchmark
{
    public class TakeSelectBenchmark
    {
        private static int[] perm = [12, 5, 27, 1, 30, 29, 4, 19, 16, 18, 31, 14, 13, 20, 6, 3, 17, 26, 22, 8, 15, 23, 10, 11, 28, 24, 21, 25, 0, 7, 9, 2];

        [Benchmark]
        public int[] TakeAndSelect()
        {
            int[] result = [.. perm.Take(perm.Length / 2).Select(n => n * n)];
            return result;
        }

        [Benchmark]
        public int[] RangeAndSelect()
        {
            int[] result = [.. perm[..(perm.Length / 2)].Select(n => n * n)];
            return result;
        }
    }
}
