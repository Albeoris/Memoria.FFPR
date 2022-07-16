namespace Memoria.CodeGenerator;

internal class ConfigConverterAttributeInfo : IConfigurationPropertyAttributeInfo
{
    public const String AttributeTypeName = "Memoria.FFPR.Configuration.Scopes.ConfigConverterAttribute";

    public String? ConverterInstance { get; private set; }

    public void SetValue(String name, String value)
    {
        switch (name)
        {
            case nameof(ConverterInstance):
            case "converterInstance":
                ConverterInstance = value;
                break;
            default:
                throw new NotSupportedException(name);
        }
    }

    public void Validate()
    {
        if (ConverterInstance is null) throw new ArgumentNullException(nameof(ConverterInstance), $"{nameof(ConverterInstance)} not initialized.");
    }

    public void Apply(ConfigurationEntryDescriptor result)
    {
        result.ConverterInstance = ConverterInstance;
    }
}