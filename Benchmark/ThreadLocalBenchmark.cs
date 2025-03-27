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
            Random seedRandom = new();
            object lockObj = new();
            return Enumerable.Range(0, N).AsParallel().Sum(_ =>
            {
                int seed;
                lock(lockObj)
                {
                    seed = seedRandom.Next();
                }
                Engine engine = new(new Random(seed));
                return (long)engine.Next();
            });
        }

        [Benchmark]
        public long EvaluateThreadLocal()
        {
            Random seedRandom = new();
            object lockObj = new();
            EngineThreadLocal engine = new();
            return Enumerable.Range(0, N).AsParallel().Sum(_ =>
            {
                int seed;
                lock (lockObj)
                {
                    seed = seedRandom.Next();
                }
                engine.SetRandom(new Random(seed));
                return (long)engine.Next();
            });
        }
    }
}
