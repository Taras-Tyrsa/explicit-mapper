using BenchmarkDotNet.Running;
using ExplicitMapper.Benchmark.SimpleMapping;
using System;

namespace ExplicitMapper.Benchmark
{
    class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
