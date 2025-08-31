using BenchmarkDotNet.Attributes;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Weaver.Benchmarks
{
    [MemoryDiagnoser(true)]
    public class MapperBenchmark
    {
        [Params(10, 50, 100, 500, 1000, 5000, 10000)]
        //[Params(2)]
        public int DataSize { get; set; }

        private DataTable _testData;

        // From the Type we can deterministckly know/get all the property namesm using reflection one time at the compile time in source generated code.
        private static readonly string[] properties = [nameof(Employee.Id), nameof(Employee.Name), nameof(Employee.Email), nameof(Employee.Designation), nameof(Employee.Compensation)];

        [GlobalSetup]
        public void Setup()
        {
            _testData = TestData.GetData(DataSize);
        }

        [Benchmark]
        public async Task<IReadOnlyList<Employee>> HandWrittenMapping_Name()
        {
            var employees = new List<Employee>(DataSize);
            DbDataReader reader = _testData.CreateDataReader();

            while (await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    Id = reader.GetInt32(nameof(Employee.Id)),
                    Name = reader.GetString(nameof(Employee.Name)),
                    Email = reader.GetString(nameof(Employee.Email)),
                    Designation = reader.GetString(nameof(Employee.Designation)),
                    Compensation = reader.GetDouble(nameof(Employee.Compensation))
                });
            }

            return employees;
        }

        [Benchmark]
        public async Task<IReadOnlyList<Employee>> HandWrittenMapping_Ordinal()
        {
            var employees = new List<Employee>(DataSize);
            DbDataReader reader = _testData.CreateDataReader();

            while (await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Designation = reader.GetString(3),
                    Compensation = reader.GetDouble(4)
                });
            }

            return employees;
        }

        [Benchmark]
        public async Task<IReadOnlyList<Employee>> ExpectedSourceGen()
        {
            var employees = new List<Employee>(DataSize);
            DbDataReader reader = _testData.CreateDataReader();

            int[] ordinal = new int[properties.Length];

            // Loop over the column and get the Ordinal poistion in DbDataReader for all properties
            for (int i = 0; i < properties.Length; i++)
            {
                try
                {
                    ordinal[i] = reader.GetOrdinal(properties[i]);
                }
                catch (IndexOutOfRangeException)
                {
                    // if property doesn't exist in the column then put -1, so that single mapper can be generated for all different sql query which maps to this type.
                    ordinal[i] = -1;
                }
            }


            while (await reader.ReadAsync())
            {
                // if we have the properties name we can loop through it and using the source generator we can generate the following code.
                // we will loop through the properties array and at the same index in ordinal array we will have ordinal position for that properties/column from DbDatReader
                employees.Add(new Employee
                {
                    // Ternary operator is used if all the properties of a type is inot in sql query and as result not in DbDataReader
                    Id = ordinal[0] >= 0 ? reader.GetInt32(ordinal[0]) : default,
                    Name = ordinal[1] >= 0 ? reader.GetString(ordinal[1]) : default,
                    Email = ordinal[2] >= 0 ? reader.GetString(ordinal[2]) : default,
                    Designation = ordinal[3] >= 0 ? reader.GetString(ordinal[3]) : default,
                    Compensation = ordinal[4] >= 0 ? reader.GetDouble(ordinal[4]) : default,
                });
            }

            return employees;
        }

        // [Benchmark]
        // public async Task<IReadOnlyList<Employee>> SourceGen()
        // {
        //     DbDataReader reader = _testData.CreateDataReader();

        //     return await EmployeeMapper.MapFromReaderAsync(reader, CancellationToken.None);
        // }

        [Benchmark]
        public IReadOnlyList<Employee> DapperMapper()
        {
            DbDataReader reader = _testData.CreateDataReader();

            var res = reader.Parse<Employee>();

            return res is List<Employee> employees ? employees : res.ToList();
        }
    }
}
