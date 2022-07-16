using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Memoria.CodeGenerator;

internal static class ConfigurationAttributeInfoFactory
{
    internal static IEnumerable<IConfigurationTypeAttributeInfo> EnumerateAttributes(ClassDeclarationSyntax type, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ConfigScopeAttributeInfo? scope = null;

        foreach ((String attributeName, AttributeSyntax attribute) in type.AttributeLists.Enumerate(context, cancellationToken))
        {
            switch (attributeName)
            {
                case ConfigScopeAttributeInfo.AttributeTypeName:
                {
                    scope ??= new ConfigScopeAttributeInfo();
                    Parse(ref scope, attribute, context, cancellationToken);
                    break;
                }
            }
        }

        if (scope is not null)
        {
            scope.Validate();
            yield return scope;
        }
        else
        {
            yield break;
        }
    }
        
    internal static IEnumerable<IConfigurationPropertyAttributeInfo> EnumerateAttributes(PropertyDeclarationSyntax property, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ConfigEntryAttributeInfo? entry = null;
        ConfigConverterAttributeInfo? converter = null;
        ConfigDependencyAttributeInfo? dependency = null;

        foreach ((String attributeName, AttributeSyntax attribute) in property.AttributeLists.Enumerate(context, cancellationToken))
        {
            switch (attributeName)
            {
                case ConfigEntryAttributeInfo.AttributeTypeName:
                {
                    entry ??= new ConfigEntryAttributeInfo();
                    Parse(ref entry, attribute, context, cancellationToken);
                    break;
                }
                case ConfigConverterAttributeInfo.AttributeTypeName:
                {
                    converter ??= new ConfigConverterAttributeInfo();
                    Parse(ref converter, attribute, context, cancellationToken);
                    break;
                }
                case ConfigDependencyAttributeInfo.AttributeTypeName:
                {
                    dependency ??= new ConfigDependencyAttributeInfo();
                    Parse(ref dependency, attribute, context, cancellationToken);
                    break;
                }
            }
        }

        if (entry is not null)
        {
            entry.Validate();
            yield return entry;
        }
        else
        {
            yield break;
        }
            
        if (converter is not null)
        {
            converter.Validate();
            yield return converter;
        }

        if (dependency is not null)
        {
            dependency.Validate();
            yield return dependency;
        }
    }

    private static void Parse<T>(ref T entry, AttributeSyntax attribute, in GeneratorSyntaxContext context, in CancellationToken cancellationToken)
        where T : IConfigurationAttributeInfo
    {
        foreach ((String name, String value) in attribute.EnumerateArguments(context, cancellationToken))
            entry.SetValue(name, value);
    }
}