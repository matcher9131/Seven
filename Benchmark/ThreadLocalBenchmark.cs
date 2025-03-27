using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class ThreadLocalBenchmark
    {
        public class Engine(Random random)
        {
            private readonly Random random = random;
            public int Next() => this.random.Next();
        }

        public class EngineThreadLocal()
        {
            private readonly ThreadLocal<Random> random = new();

            public void SetRandom(Random random) => this.random.Value = random;
            public int Next() => this.random.Value?.Next() ?? throw new InvalidOperationException();
        }

        const int N = 100000;

        [Benchmark]
        public long Evaluate()
        {
            return Enumerable.Range(0, N).AsParallel().Sum(_ =>
            {
                Engine engine = new(new Random(Random.Shared.Next()));
                return (long)engine.Next();
            });
        }

        [Benchmark]
        public long EvaluateThreadLocal()
        {
            EngineThreadLocal engine = new();
            return Enumerable.Range(0, N).AsParallel().Sum(_ =>
            {
                engine.SetRandom(new Random(Random.Shared.Next()));
                return (long)engine.Next();
            });
        }
    }
}
