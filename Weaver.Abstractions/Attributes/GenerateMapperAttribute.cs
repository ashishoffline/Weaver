namespace Weaver.Abstractions.Attributes;

/// <summary>
/// Marks a class for which a data mapper should be automatically generated.
/// </summary>
/// <remarks>
/// This attribute should be applied to entity or DTO classes.  
/// A source generator or runtime component can use this marker to generate
/// mapping code (e.g., mapping database rows to strongly typed objects).
/// <para>
/// The <see cref="IsStrict"/> property specifies whether all properties in the class 
/// must have corresponding columns in the query result.  
/// When set to <c>true</c>:
/// <list type="bullet">
/// <item>
/// <description>
/// The generated mapper will throw a <see cref="IndexOutOfRangeException"/>  
/// if any property does not have a matching column in the result set.
/// </description>
/// </item>
/// <item>
/// <description>
/// A small performance gain can be achieved, since the mapper does not need to 
/// perform additional existence or null checks for missing columns.
/// </description>
/// </item>
/// </list>
/// By default, <see cref="IsStrict"/> is <c>false</c>.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateMapperAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether all properties in the class should be treated as strict,
    /// meaning they are guaranteed to be existed when retrieved from the database.
    /// </summary>
    public bool IsStrict { get; init; }
}
