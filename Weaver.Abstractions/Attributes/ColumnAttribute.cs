namespace Weaver.Abstractions.Attributes;

/// <summary>
/// Specifies metadata for mapping a property to a database column.
/// </summary>
/// <remarks>
/// This attribute is applied to entity or DTO properties to explicitly define the name 
/// of the corresponding database column.  
/// <para>
/// It is only required when the property name differs from the database column name.  
/// If the property name and column name are identical, the attribute can be omitted.
/// </para>
/// <para>
/// This attribute is typically consumed by source generators or mapping utilities to 
/// automate the creation of strongly typed mappers.
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="Name"/>: The exact name of the database column.  
/// This value is used when generating mapping code 
/// (e.g., when reading values from a <see cref="System.Data.Common.DbDataReader"/>).
/// </description>
/// </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ColumnAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the database column associated with the property.  
    /// This value is required when the property name does not match the column name.
    /// </summary>
    public required string Name { get; init; }
}

