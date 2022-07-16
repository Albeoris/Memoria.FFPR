using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Memoria.CodeGenerator;

internal sealed class ConfigurationEntryDescriptor : IEquatable<ConfigurationEntryDescriptor>
{
    public TypeSyntax Type { get; }
    public String Name { get; }
    public Boolean HasSetter { get; set; }
    public Optional<Object?> DefaultValue { get; set; }
    
    public String? Description { get; set; }
    public String? ConverterInstance { get; set; }
    public ConfigDependencyAttributeInfo? Dependency { get; set; }

    public ConfigurationEntryDescriptor(TypeSyntax type, String name)
    {
        Type = type;
        Name = name;
    }

    public String BackingFieldName => $"_{Char.ToLower(Name[0], CultureInfo.InvariantCulture)}{Name.Substring(1)}";


    public override Boolean Equals(Object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ConfigurationEntryDescriptor other && Equals(other);
    }

    public Boolean Equals(ConfigurationEntryDescriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Type.Equals(other.Type)
               && Name == other.Name
               && HasSetter == other.HasSetter
               && DefaultValue.Equals(other.DefaultValue)
               && Description == other.Description
               && ConverterInstance == other.ConverterInstance
               && Equals(Dependency, other.Dependency);
    }

    public override Int32 GetHashCode()
    {
        unchecked
        {
            var hashCode = Type.GetHashCode();
            hashCode = (hashCode * 397) ^ Name.GetHashCode();
            hashCode = (hashCode * 397) ^ HasSetter.GetHashCode();
            hashCode = (hashCode * 397) ^ DefaultValue.GetHashCode();
            hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (ConverterInstance != null ? ConverterInstance.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Dependency != null ? Dependency.GetHashCode() : 0);
            return hashCode;
        }
    }
}