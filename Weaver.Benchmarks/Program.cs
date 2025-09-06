using BenchmarkDotNet.Running;

namespace Weaver.Benchmarks
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MapperBenchmark>();
        }
    }
}
