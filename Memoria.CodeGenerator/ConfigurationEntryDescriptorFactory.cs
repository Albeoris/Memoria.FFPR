using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Memoria.CodeGenerator;

internal static class ConfigurationEntryDescriptorFactory
{
    public static ConfigurationEntryDescriptor? TryCreateEntry(PropertyDeclarationSyntax property, in GeneratorSyntaxContext context, in CancellationToken cancellationToken)
    {
        ConfigurationEntryDescriptor? result = TryCreatePropertyDescriptor(property, context, cancellationToken);
        if (result is null)
            return null;

        // Default value
        if (property.Initializer is not null)
            result.DefaultValue = property.Initializer.Value;
        else if (property.ExpressionBody is not null)
            result.DefaultValue = property.ExpressionBody.Expression.ToString();

        // Modifiers
        if (!property.Modifiers.Has(SyntaxKind.VirtualKeyword))
            throw new NotSupportedException($"Property {property} is not virtual.");

        // Accessors
        (Boolean hasGetter, Boolean hasSetter) = property.GetAccessors();
        if (!hasGetter)
            throw new NotSupportedException($"Property {property} has no getter.");
        result.HasSetter = hasSetter;

        return result;
    }

    private static ConfigurationEntryDescriptor? TryCreatePropertyDescriptor(PropertyDeclarationSyntax property, in GeneratorSyntaxContext context, in CancellationToken cancellationToken)
    {
        ConfigurationEntryDescriptor? result = null;
        foreach (IConfigurationPropertyAttributeInfo info in ConfigurationAttributeInfoFactory.EnumerateAttributes(property, context, cancellationToken))
        {
            result ??= new ConfigurationEntryDescriptor(property.Type, property.Identifier.ToString());
            info.Apply(result);
        }

        return result;
    }
}