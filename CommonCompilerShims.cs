#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices;

// Shim for init accessor
internal sealed class IsExternalInit { }

// Shim for required members

/// <summary>
/// Marks fields/properties as required.
/// Required for required keyword.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
internal sealed class RequiredMemberAttribute : Attribute { }

/// <summary>
/// Marks constructors that satisfy required members.
/// Required for object initializers.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor, Inherited = false)]
internal sealed class SetsRequiredMembersAttribute : Attribute { }

/// <summary>
/// Tells the compiler that a feature (like required) is in use.
/// Required for compiler enforcement.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;
    public string FeatureName { get; }
    public bool IsOptional { get; init; }
}
#endif
