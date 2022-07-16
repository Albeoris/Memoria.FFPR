namespace Memoria.CodeGenerator;

internal interface IConfigurationTypeAttributeInfo : IConfigurationAttributeInfo
{
    void Apply(ConfigurationScopeDescriptor result);
}