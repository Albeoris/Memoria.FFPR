namespace Memoria.CodeGenerator;

internal sealed class ConfigEntryAttributeInfo : IConfigurationPropertyAttributeInfo
{
    public const String AttributeTypeName = "Memoria.FFPR.Configuration.Scopes.ConfigEntryAttribute";

    public String? Description { get; private set; }

    public void SetValue(String name, String value)
    {
        switch (name)
        {
            case nameof(Description):
            case "description":
                Description = value;
                break;
            default:
                throw new NotSupportedException(name);
        }
    }

    public void Validate()
    {
        if (Description is null) throw new ArgumentNullException(nameof(Description), $"{nameof(Description)} not initialized.");
    }

    public void Apply(ConfigurationEntryDescriptor result)
    {
        result.Description = Description;
    }
}