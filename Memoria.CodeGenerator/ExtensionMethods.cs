using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Memoria.CodeGenerator;

internal static class ExtensionMethods
{
    public static IEnumerable<(String name, String value)> EnumerateArguments(this AttributeSyntax attribute, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        SeparatedSyntaxList<AttributeArgumentSyntax>? arguments = attribute.ArgumentList?.Arguments;
        if (arguments is null)
            yield break;

        IMethodSymbol? constructor = context.SemanticModel.GetSymbolInfo(attribute).Symbol as IMethodSymbol;

        Int32 index = -1;
        foreach (AttributeArgumentSyntax arg in arguments)
        {
            index++;
            String name = arg.NameEquals?.Name.Identifier.ToString() // Named properties
                          ?? constructor!.Parameters[index].Name;    // Constructut arguments 
            
            String value = context.SemanticModel.GetConstantValue(arg.Expression, cancellationToken).ToString();
            yield return (name, value);
        }
    }

    public static String GetFullName(this BaseNamespaceDeclarationSyntax ns)
    {
        if (ns.Parent is not BaseNamespaceDeclarationSyntax)
            return ns.Name.ToString();

        Int32 size = 0;
        Stack<String> stack = new();
        BaseNamespaceDeclarationSyntax? current = ns;
        while (current != null)
        {
            String currentName = current.Name.ToString();
            size += currentName.Length;
            stack.Push(currentName);
            current = current.Parent as BaseNamespaceDeclarationSyntax;
        }

        StringBuilder sb = new(capacity: size + stack.Count);
        while (stack.Count > 0)
        {
            sb.Append(stack.Pop());
            sb.Append('.');
        }

        return sb.ToString(0, sb.Length - 1);
    }

    public static Boolean TryResolveNamespace(this ClassDeclarationSyntax cds, out String nameSpace)
    {
        if (cds.Parent is not BaseNamespaceDeclarationSyntax ns)
        {
            if (cds.Parent is not CompilationUnitSyntax)
            {
                // Nested types are not supported
                nameSpace = null!;
                return false;
            }

            nameSpace = String.Empty;
            return true;
        }

        nameSpace = ns.GetFullName();
        return true;
    }

    public static IEnumerable<PropertyDeclarationSyntax> EnumerateProperties(this ClassDeclarationSyntax cds, CancellationToken cancellationToken)
    {
        foreach (PropertyDeclarationSyntax? property in cds.Members.OfType<PropertyDeclarationSyntax>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return property;
        }
    }

    public static IEnumerable<(String Name, AttributeSyntax Syntax)> Enumerate(this SyntaxList<AttributeListSyntax> attributes, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        return Enumerate(attributes.SelectMany(l => l.Attributes), context, cancellationToken);
    }

    public static IEnumerable<(String Name, AttributeSyntax Syntax)> Enumerate(this IEnumerable<AttributeSyntax> attributes, GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        SemanticModel model = context.SemanticModel;

        foreach (AttributeSyntax attribute in attributes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            if (ModelExtensions.GetSymbolInfo(model, attribute, cancellationToken).Symbol is not IMethodSymbol caSymbol)
            {
                // Invalid attribute defination
                continue;
            }

            String currentName = caSymbol.ContainingType.ToDisplayString();
            yield return (currentName, attribute);
        }
    }

    public static Boolean Has(this SyntaxTokenList tokenList, SyntaxKind kind)
    {
        return tokenList.Any(token => token.IsKind(kind));
    }

    public static (Boolean hasGetter, Boolean hasSetter) GetAccessors(this PropertyDeclarationSyntax property)
    {
        Boolean hasGetter = false;
        Boolean hasSetter = false;
        
        SyntaxList<AccessorDeclarationSyntax>? accessors = property.AccessorList?.Accessors;
        if (accessors != null)
        {
            foreach (AccessorDeclarationSyntax accessor in accessors)
            {
                if (accessor.Keyword.IsKind(SyntaxKind.GetAccessorDeclaration) || accessor.Keyword.IsKind(SyntaxKind.GetKeyword))
                    hasGetter = true;
                else if (accessor.Keyword.IsKind(SyntaxKind.SetAccessorDeclaration) || accessor.Keyword.IsKind(SyntaxKind.SetKeyword))
                    hasSetter = true;
                else
                    throw new NotSupportedException(accessor.Keyword.ToFullString());
            }
        }
        else
        {
            hasGetter = true;
        }

        return (hasGetter, hasSetter);
    }
}