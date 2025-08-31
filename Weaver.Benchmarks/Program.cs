using BenchmarkDotNet.Running;
using System.Data.Common;

namespace Weaver.Benchmarks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            BenchmarkRunner.Run<MapperBenchmark>();
        }
    }
}
