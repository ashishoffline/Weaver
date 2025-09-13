namespace Weaver.Abstractions.Attributes;

///<summary>
/// Marks a class for which a data mapper should be automatically generated.
/// </summary>
/// <remarks>
/// This attribute is intended to be applied to entity or DTO classes.
/// A source generator or runtime component can use this marker to generate
/// mapping code (e.g., between database rows and strongly typed objects).
/// <para>
/// The <see cref="IsStrict"/> property allows the mapper to treat all columns as non-nullable,
/// skipping null checks during mapping. By default, <see cref="IsStrict"/> is <c>false</c>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateMapperAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether all properties in the class should be treated as strict,
    /// meaning they are guaranteed to be non-null when retrieved from the database.
    /// </summary>
    public bool IsStrict { get; set; }
}
