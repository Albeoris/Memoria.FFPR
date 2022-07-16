namespace Memoria.CodeGenerator;

internal interface IConfigurationPropertyAttributeInfo : IConfigurationAttributeInfo
{
    void Apply(ConfigurationEntryDescriptor result);
}