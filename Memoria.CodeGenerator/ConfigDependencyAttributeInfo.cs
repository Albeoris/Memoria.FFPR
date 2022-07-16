namespace Memoria.CodeGenerator;

internal sealed class ConfigDependencyAttributeInfo : IConfigurationPropertyAttributeInfo, IEquatable<ConfigDependencyAttributeInfo>
{
    public const String AttributeTypeName = "Memoria.FFPR.Configuration.Scopes.ConfigDependencyAttribute";

    public String? PropertyName { get; private set; }
    public String? DefaultValue { get; private set; }

    public void SetValue(String name, String value)
    {
        switch (name)
        {
            case nameof(PropertyName):
            case "propertyName":
                PropertyName = value;
                break;
            case nameof(DefaultValue):
            case "defaultValue":
                DefaultValue = value;
                break;
            default:
                throw new NotSupportedException(name);
        }
    }

    public void Validate()
    {
        if (PropertyName is null) throw new ArgumentNullException(nameof(PropertyName), $"{nameof(PropertyName)} not initialized.");
        if (DefaultValue is null) throw new ArgumentNullException(nameof(DefaultValue), $"{nameof(DefaultValue)} not initialized.");
    }

    public void Apply(ConfigurationEntryDescriptor result)
    {
        result.Dependency = this;
    }

    public override Boolean Equals(Object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ConfigDependencyAttributeInfo other && Equals(other);
    }

    public Boolean Equals(ConfigDependencyAttributeInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return PropertyName == other.PropertyName && DefaultValue == other.DefaultValue;
    }

    public override Int32 GetHashCode()
    {
        unchecked
        {
            return ((PropertyName != null ? PropertyName.GetHashCode() : 0) * 397) ^ (DefaultValue != null ? DefaultValue.GetHashCode() : 0);
        }
    }
}