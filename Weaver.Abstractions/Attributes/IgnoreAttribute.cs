namespace Weaver.Abstractions.Attributes;

using System;

/// <summary>
/// Specifies that a property should be ignored by mapping or persistence logic.
/// </summary>
/// <remarks>
/// Use this attribute when a property exists on a class but should not be included 
/// in database mapping, serialization, or code generation processes.
/// <para>
/// Common use cases:
/// <list type="bullet">
///   <item>
///     <description>
///     Preventing a property from being mapped to a database column.
///     </description>
///   </item>
///   <item>
///     <description>
///     Excluding a property from auto-generated mappers (e.g., when using 
///     <see cref="GenerateMapperAttribute"/>).
///     </description>
///   </item>
///   <item>
///     <description>
///     Skipping sensitive or calculated properties during persistence.
///     </description>
///   </item>
/// </list>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class IgnoreAttribute : Attribute
{
}

