using BenchmarkDotNet.Running;
using System.Data.Common;

namespace Weaver.Benchmarks
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //DbDataReader reader = TestData.GetData(2).CreateDataReader();

            //var res = await EmployeeMapper.MapFromReaderAsync(reader, CancellationToken.None);

            BenchmarkRunner.Run<MapperBenchmark>();
        }
    }
}
