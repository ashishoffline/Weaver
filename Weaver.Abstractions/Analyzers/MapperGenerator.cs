using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Data.Common;
using System.Text;

namespace Weaver.Abstractions.Analyzers;

/// <summary>
/// A Roslyn source generator that automatically generates mapper classes for 
/// entity or DTO classes marked with <see cref="Weaver.Abstractions.Attributes.GenerateMapperAttribute"/>.
/// </summary>
/// <remarks>
/// This class implements <see cref="IIncrementalGenerator"/>, allowing it to participate 
/// in incremental source generation for better performance and scalability. 
/// <para>
/// The generator scans the compilation for classes decorated with <see cref="Weaver.Abstractions.Attributes.GenerateMapperAttribute"/> 
/// and generates strongly-typed mapping code (e.g., from <see cref="DbDataReader"/> to the class).
/// </para>
/// <para>
/// Features:
/// <list type="bullet">
///   <item>
///     <description>Supports optional <c>IsStrict</c> behavior for null-checking.</description>
///   </item>
///   <item>
///     <description>Precomputes column ordinals for efficient database mapping.</description>
///   </item>
///   <item>
///     <description>Generates asynchronous mapping methods for single or multiple rows.</description>
///   </item>
/// </list>
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public class MapperGenerator : IIncrementalGenerator
{
    private sealed class PropertyInfo
    {
        public required string Name { get; init; }
        public required string Type { get; init; }
        public required string ColumnName { get; init; }
        public required bool IsNullable { get; init; }
        public required bool IsInitOnly { get; init; }
    }

    /// <summary>
    /// Initializes the source generator by registering incremental steps for code generation.
    /// </summary>
    /// <param name="context">
    /// The <see cref="IncrementalGeneratorInitializationContext"/> provided by the compiler, 
    /// used to register syntax providers, transformations, and outputs for the generator.
    /// </param>
    /// <remarks>
    /// This method is called by the compiler once during the generation process. 
    /// It should be used to set up the incremental pipeline, including:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Detecting relevant syntax nodes or symbols (e.g., classes with <see cref="Weaver.Abstractions.Attributes.GenerateMapperAttribute"/>).</description>
    ///   </item>
    ///   <item>
    ///     <description>Transforming input nodes or symbols into generator models.</description>
    ///   </item>
    ///   <item>
    ///     <description>Registering outputs that will produce generated source code.</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Collect candidate classes with [GenerateMapper]
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                transform: static (ctx, _) =>
                {
                    var classDecl = (ClassDeclarationSyntax)ctx.Node;
                    var symbol = ctx.SemanticModel.GetDeclaredSymbol(classDecl, _);
                    if (symbol is INamedTypeSymbol namedType &&
                        namedType.GetAttributes().Any(attr => attr.AttributeClass?.Name == "GenerateMapperAttribute"))
                    {
                        return namedType;
                    }
                    return null;
                })
            .Where(static symbol => symbol is not null);

        // Step 2: Generate source for each class
        context.RegisterSourceOutput(classDeclarations, (spc, classSymbol) =>
        {
            var source = GenerateMapperForClass(classSymbol!);
            var fileName = $"{classSymbol!.Name}.Mapper.g.cs";
            spc.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
        });
    }

    #region PRIVATE METHODS
    private static string GenerateMapperForClass(INamedTypeSymbol classSymbol)
    {
        var properties = GetMappableProperties(classSymbol);
        var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
        var className = classSymbol.Name;

        // Since the attribute is guaranteed to exist, we can assume First will not throw exception.
        AttributeData generateMapperAttribute= classSymbol.GetAttributes()
            .First(ad => ad.AttributeClass?.Name == "GenerateMapperAttribute");
        bool isClassStrict = GetClassIsStrict(generateMapperAttribute);

        return GenerateMappingCode(properties, namespaceName, className, isClassStrict);
    }

    private static bool GetClassIsStrict(AttributeData attribute)
    {
        // If there is exactly one named argument, and it is "IsStrict", return its value
        if (attribute.NamedArguments.Length == 1 &&
            attribute.NamedArguments[0].Key == "IsStrict" &&
            attribute.NamedArguments[0].Value.Value is bool isStrict)
        {
            return isStrict;
        }

        // Default value if not specified
        return false;
    }
    
    private static string GenerateMappingCode(List<PropertyInfo> properties, string namespaceName, string className, bool isStrict)
    {
        var ordinalFunctionName = isStrict ? "GetOrdinal" : "GetOrdinalSafe";
        var ordinalVariables = string.Join($";\n\t\t\t", properties.Select(prop => $"int ord{prop.Name} = reader.{ordinalFunctionName}(\"{prop.ColumnName}\")"));

        // Generate property initializers for object creation
        var propertyMappings = string.Join(",\n\t\t\t\t\t", properties.Select((prop, index) =>
        {
            return isStrict
                    ? $"{prop.Name} = reader.{GetValueFunction(prop.Type, prop.IsNullable)}(ord{prop.Name})"
                    : $"{prop.Name} = ord{prop.Name} >= 0 ? reader.{GetValueFunction(prop.Type, prop.IsNullable)}(ord{prop.Name}) : default";
        }));

        return $$"""
// Source Generated code, don't modify.
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Weaver.Abstractions;

namespace {{namespaceName}}
{
    [GeneratedCode("Weaver.Abstractions", "1.0.0")]
    public sealed class {{className}}Mapper : IDbDataReaderMapper<{{className}}>
    {
        ///<inheritdoc/>
        public async Task<IReadOnlyList<{{className}}>> MapAllFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            var results = new List<{{className}}>();
            // Pre-calculate ordinals for each column
            {{ordinalVariables}};
            
            // Process all rows using pre-calculated ordinals
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                results.Add(new {{className}}
                {
                    {{propertyMappings}}
                });
            }
            
            return results.AsReadOnly();
        }
        
        ///<inheritdoc/>
        public async Task<{{className}}?> MapFirstFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            {{className}}? result = null;
            // Pre-calculate ordinals for each column
            {{ordinalVariables}};
            
            // Process all rows using pre-calculated ordinals
            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                result = new {{className}}
                {
                    {{propertyMappings}}
                };
            }
            
            return result;
        }

        ///<inheritdoc/>
        public async IAsyncEnumerable<{{className}}> StreamFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            // Pre-calculate ordinals for each column
            {{ordinalVariables}};
            
            // Process all rows using pre-calculated ordinals
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return new {{className}}
                {
                    {{propertyMappings}}
                };
            }
        }
    }
}
""";
    }

    private static List<PropertyInfo> GetMappableProperties(INamedTypeSymbol typeSymbol)
    {
        var properties = new List<PropertyInfo>();

        foreach (var member in typeSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            // Support both { get; set; } and { get; init; } properties
            if (member.SetMethod == null && member.GetMethod?.DeclaredAccessibility != Accessibility.Public)
                continue; // Skip read-only properties without init

            // Skip properties with [Ignore] attribute
            if (member.GetAttributes().Any(attr => attr.AttributeClass?.Name == "IgnoreAttribute"))
                continue;

            string columnName = GetColumnName(member);

            bool isInitOnly = member.SetMethod?.IsInitOnly == true;

            properties.Add(new PropertyInfo
            {
                Name = member.Name,
                Type = member.Type.ToDisplayString(),
                ColumnName = columnName,
                IsNullable = member.Type.CanBeReferencedByName && (member.Type.IsReferenceType ||
                (member.Type is INamedTypeSymbol { IsGenericType: true } namedType &&
                 namedType.ConstructedFrom.ToDisplayString() == "System.Nullable<T>")),
                IsInitOnly = isInitOnly
            });
        }

        return properties;
    }

    private static string GetColumnName(IPropertySymbol property)
    {
        var columnAttr = property.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.Name == "ColumnAttribute");

        if (columnAttr?.ConstructorArguments.Length > 0)
        {
            return columnAttr.ConstructorArguments[0].Value?.ToString() ?? property.Name;
        }

        return property.Name;
    }

    private static string GetValueFunction(string typeName, bool isNullable)
    {
        return typeName switch
        {
            "int" => nameof(DbDataReader.GetInt32),
            "int?" => nameof(DataReaderExtensions.GetNullableInt32),
            "double" => nameof(DbDataReader.GetDouble),
            "double?" => nameof(DataReaderExtensions.GetNullableDouble),
            "string" or "string?" => nameof(DataReaderExtensions.GetNullableString),
            _ => nameof(DbDataReader.GetValue),
        };
    }
    
    #endregion
}

