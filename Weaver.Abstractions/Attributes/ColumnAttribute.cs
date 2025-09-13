namespace Weaver.Abstractions.Attributes;

/// <summary>
/// Specifies metadata for mapping a property to a database column.
/// </summary>
/// <remarks>
/// This attribute is applied to entity or DTO properties to define their corresponding 
/// database column names. It is intended for use by source generators or mapping utilities 
/// to automate the creation of mappers.
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="Name"/>: The name of the database column.  
/// This value is directly used when generating mapping code 
/// (e.g., when reading values from <c>DbDataReader</c>).
/// </description>
/// </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class ColumnAttribute : Attribute
{
    /// <summary>
    /// Gets the value which is used by code generators and mapping logic to 
    /// resolve the correct column name at runtime.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class
    /// with the specified column name.
    /// </summary>
    /// <param name="name">
    /// This name is used directly in mapper generation.
    /// </param>
    public ColumnAttribute(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
