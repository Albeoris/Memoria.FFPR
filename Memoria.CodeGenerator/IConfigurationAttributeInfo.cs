namespace Memoria.CodeGenerator;

internal interface IConfigurationAttributeInfo
{
    void SetValue(String name, String value);
    public void Validate();
}