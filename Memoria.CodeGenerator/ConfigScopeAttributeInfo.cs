namespace Memoria.CodeGenerator;

internal sealed class ConfigScopeAttributeInfo : IConfigurationTypeAttributeInfo
{
    public const String AttributeTypeName = "Memoria.FFPR.Configuration.Scopes.ConfigScopeAttribute";

    public String? SectionName { get; private set; }

    public void SetValue(String name, String value)
    {
        switch (name)
        {
            case nameof(SectionName):
            case "sectionName":
                SectionName = value;
                break;
            default:
                throw new NotSupportedException(name);
        }
    }
        
    public void Validate()
    {
        if (SectionName is null) throw new ArgumentNullException(nameof(SectionName), $"{nameof(SectionName)} not initialized.");
    }

    public void Apply(ConfigurationScopeDescriptor result)
    {
        result.SectionName = SectionName;
    }
}