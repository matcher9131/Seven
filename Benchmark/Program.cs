﻿using Benchmark;
using BenchmarkDotNet.Running;

// var summary = BenchmarkRunner.Run<TakeSelectBenchmark>();
// var summary = BenchmarkRunner.Run<OrderBasedCrossoverBenchmark>();
var summary = BenchmarkRunner.Run<ThreadLocalBenchmark>();