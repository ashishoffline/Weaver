using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Weaver.Abstractions.Analyzers
{
    [Generator(LanguageNames.CSharp)]
    public class MapperGenerator : IIncrementalGenerator
    {
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
                .Where(static symbol => symbol is not null)!;

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
            var mappingCode = GenerateMappingCode(properties, className);

            return $@"
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
        {mappingCode}
    }}
}}
";
        }

        private string GenerateMappingCode(List<PropertyInfo> properties, string className)
        {
            var sb = new StringBuilder();

            // Generate the column names array(compile-time constant)
            sb.AppendLine("private static readonly string[] _columnNames = new string[] { ");
            foreach (var property in properties)
            {
                sb.Append($"\"{property.ColumnName}\",");
            }
            sb.AppendLine(" };");
            sb.AppendLine();

            // Generate the optimized multi-row mapper
            sb.AppendLine($"public static async Task<IReadOnlyList<{className}>> MapFromReaderAsync(DbDataReader reader, CancellationToken cancellationToken)");
            sb.AppendLine("{");

            sb.AppendLine($"var results = new List<{className}>();");

            sb.AppendLine("// Get column ordinals once for the entire result set");
            sb.AppendLine("int[] propertyPositionInReader = new int[_columnNames.Length];");
            sb.AppendLine($"for (int i = 0; i < _columnNames.Length; i++)");
            sb.AppendLine("{");
            sb.AppendLine("propertyPositionInReader[i] = reader.GetOrdinalSafe(_columnNames[i]);");
            sb.AppendLine("}");

            sb.AppendLine();
            sb.AppendLine("int ordinal;");
            sb.AppendLine("// Process all rows using pre-calculated ordinals");
            sb.AppendLine("while(await reader.ReadAsync(cancellationToken))");
            sb.AppendLine("{");

            sb.AppendLine($"results.Add(new {className}");
            sb.AppendLine("{");
            // Generate object initializer with ordinal array access
            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];

                sb.AppendLine($"{prop.Name} = propertyPositionInReader.TryGetValidOrdinal({i}, out ordinal) ? ({prop.Type})reader.GetValue(ordinal) : default,");
            }
            sb.AppendLine("});");

            sb.AppendLine("}");

            sb.AppendLine("return results.AsReadOnly();");
            sb.AppendLine("}");

            return sb.ToString();
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

        private class PropertyInfo
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string ColumnName { get; set; }
            public bool IsNullable { get; set; }
            public bool IsInitOnly { get; set; }
        }

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<INamedTypeSymbol> CandidateClasses { get; } = new List<INamedTypeSymbol>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0)
                {
                    var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
                    if (classSymbol != null && classSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == "GenerateMapperAttribute"))
                    {
                        CandidateClasses.Add(namedTypeSymbol);
                    }
                }
            }
        }

        //private string GetValueFunction(string typeName)
        //{
        //    return typeName switch
        //    {
        //        "int" => 
        //    };
        //}
    }
}

