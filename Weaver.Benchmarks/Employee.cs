using Weaver.Abstractions.Attributes;

namespace Weaver.Benchmarks;

[GenerateMapper]
public partial class Employee
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Designation { get; init; }
    public double Compensation { get; init; }
}