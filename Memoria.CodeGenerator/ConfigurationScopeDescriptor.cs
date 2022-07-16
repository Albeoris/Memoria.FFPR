using Microsoft.CodeAnalysis;

namespace Memoria.CodeGenerator;

internal sealed class ConfigurationScopeDescriptor : IEquatable<ConfigurationScopeDescriptor>
{
    public String Namespace { get; }
    public String TypeName { get; }
    public SyntaxTokenList Modifiers { get; }

    public String? SectionName { get; set; }

    public IReadOnlyList<ConfigurationEntryDescriptor>? Properties { get; set; }

    public ConfigurationScopeDescriptor(String ns, String typeName, SyntaxTokenList modifiers)
    {
        Namespace = ns;
        TypeName = typeName;
        Modifiers = modifiers;
    }

    public String ImplementationTypeName => $"{TypeName}Impl";

    public override Boolean Equals(Object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ConfigurationScopeDescriptor other && Equals(other);
    }
    
    public Boolean Equals(ConfigurationScopeDescriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Namespace == other.Namespace
               && TypeName == other.TypeName
               && Modifiers.Equals(other.Modifiers)
               && SectionName == other.SectionName
               && Properties.SequenceEqual(other.Properties);
    }

    public override Int32 GetHashCode()
    {
        unchecked
        {
            var hashCode = Namespace?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ TypeName?.GetHashCode() ?? 0;
            hashCode = (hashCode * 397) ^ Modifiers.GetHashCode();
            hashCode = (hashCode * 397) ^ SectionName?.GetHashCode() ?? 0;
            foreach (var item in Properties!)
                hashCode = (hashCode * 397) ^ item?.GetHashCode() ?? 0;
            return hashCode;
        }
    }
}