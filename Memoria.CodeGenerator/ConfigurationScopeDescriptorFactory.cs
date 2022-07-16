using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Memoria.CodeGenerator;

internal static class ConfigurationScopeDescriptorFactory
{
    public static Boolean IsSuitableNode(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    public static ConfigurationScopeDescriptor? TryCreateScope(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ClassDeclarationSyntax type = (ClassDeclarationSyntax)context.Node;
        
        ConfigurationScopeDescriptor? result = TryCreateTypeDescriptor(type, context, cancellationToken);
        if (result is null)
            return null;

        result.Properties = CreatePropertyDescriptors(type, context, cancellationToken);
        return result;
    }

    private static List<ConfigurationEntryDescriptor> CreatePropertyDescriptors(ClassDeclarationSyntax type, in GeneratorSyntaxContext context, in CancellationToken cancellationToken)
    {
        List<ConfigurationEntryDescriptor> entries = new();
        foreach (PropertyDeclarationSyntax property in type.EnumerateProperties(cancellationToken))
        {
            ConfigurationEntryDescriptor? entry = ConfigurationEntryDescriptorFactory.TryCreateEntry(property, context, cancellationToken);
            if (entry is not null)
                entries.Add(entry);
        }

        return entries;
    }

    private static ConfigurationScopeDescriptor? TryCreateTypeDescriptor(ClassDeclarationSyntax type, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        ConfigurationScopeDescriptor? result = null;
        
        foreach (IConfigurationTypeAttributeInfo info in ConfigurationAttributeInfoFactory.EnumerateAttributes(type, context, cancellationToken))
        {
            if (result is null)
            {
                if (!type.TryResolveNamespace(out String nameSpace))
                    return result;

                String typeName = type.Identifier.ToString();
                result = new ConfigurationScopeDescriptor(nameSpace, typeName, type.Modifiers);
            }

            info.Apply(result);
        }

        return result;
    }
}