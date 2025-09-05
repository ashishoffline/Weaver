using BenchmarkDotNet.Running;

namespace Weaver.Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MapperBenchmark>();
        }
    }
}
