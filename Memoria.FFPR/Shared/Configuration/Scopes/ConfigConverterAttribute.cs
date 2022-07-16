using System;

namespace Memoria.FFPR.Configuration.Scopes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class ConfigConverterAttribute : Attribute
{
    public String ConverterInstance { get; }
    
    public ConfigConverterAttribute(String converterInstance)
    {
        ConverterInstance = converterInstance;
    }
}