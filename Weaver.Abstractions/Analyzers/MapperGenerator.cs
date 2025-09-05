using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Weaver.Abstractions.Analyzers
{
    [Generator(LanguageNames.CSharp)]
    public class MapperGenerator : IIncrementalGenerator
    {
        private class PropertyInfo
        {
            public string Name { get; init; }
            public string Type { get; init; }
            public string ColumnName { get; init; }
            public bool IsNullable { get; init; }
            public bool IsInitOnly { get; init; }
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Step 1: Collect candidate classes with [GenerateMapper]
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                    transform: static (ctx, _) =>
                    {
                        var classDecl = (ClassDeclarationSyntax)ctx.Node;
                        var symbol = ctx.SemanticModel.GetDeclaredSymbol(classDecl);
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

        private string GenerateMapperForClass(INamedTypeSymbol classSymbol)
        {
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
            var className = classSymbol.Name;

            var properties = GetMappableProperties(classSymbol);
            return GenerateMappingCode(properties, namespaceName, className, true);
        }

        private string GenerateMappingCode(List<PropertyInfo> properties, string namespaceName, string className, bool isStrict)
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

            return $@"// Source Generated code, don't modify.
using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Weaver.Abstractions.Extensions;

namespace {namespaceName}
{{
    public static class {className}Mapper
    {{
        public static async Task<IReadOnlyList<{className}>> MapFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken)
        {{
            var results = new List<{className}>();

            // Ordinal Values for each column.
            {ordinalVariables};
            
            // Process all rows using pre-calculated ordinals
            while (await reader.ReadAsync(cancellationToken))
            {{
                results.Add(new {className}
                {{
                    {propertyMappings}
                }});
            }}
            
            return results.AsReadOnly();
        }}
    }}
}}";
        }

        private List<PropertyInfo> GetMappableProperties(INamedTypeSymbol typeSymbol)
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

                var columnName = GetColumnName(member);

                var isInitOnly = member.SetMethod?.IsInitOnly == true;

                properties.Add(new PropertyInfo
                {
                    Name = member.Name,
                    Type = member.Type.ToDisplayString(),
                    ColumnName = columnName,
                    IsNullable = member.Type.CanBeReferencedByName && (member.Type.IsReferenceType ||
                    (member.Type is INamedTypeSymbol namedType &&
                    namedType.IsGenericType &&
                    namedType.ConstructedFrom.ToDisplayString() == "System.Nullable<T>")),
                    IsInitOnly = isInitOnly
                });
            }

            return properties;
        }

        private string GetColumnName(IPropertySymbol property)
        {
            var columnAttr = property.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == "ColumnAttribute");

            if (columnAttr?.ConstructorArguments.Length > 0)
            {
                return columnAttr.ConstructorArguments[0].Value?.ToString() ?? property.Name;
            }

            return property.Name;
        }

        private string GetValueFunction(string typeName, bool isNullable)
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
    }
}

