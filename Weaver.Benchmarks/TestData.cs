using System.Data;

namespace Weaver.Benchmarks;

internal static class TestData
{
    public static DataTable GetData(int size = 10000)
    {
        DataTable dt = new DataTable("TestData");
        dt.Columns.AddRange(
        [
            new DataColumn("Id", typeof(int)),
            new DataColumn("Name", typeof(string)),
            new DataColumn("Email", typeof(string)),
            new DataColumn("Designation", typeof(string)),
            new DataColumn("Compensation", typeof(double))
        ]);

        for (int i = 0; i < size; i++)
        {
            dt.Rows.Add(
                i + 1, // Sequential for consistency
                Guid.NewGuid().ToString("N")[..12],
                $"user{i}@test.com", // Consistent pattern
                Guid.NewGuid().ToString("N")[..8],
                Random.Shared.NextDouble() * 100000 + 50000
            );
        }

        return dt;
    }
}
