using System;

namespace Memoria.FFPR.Configuration.Scopes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ConfigScopeAttribute : Attribute
{
    public String SectionName { get; }
    
    public ConfigScopeAttribute(String sectionName)
    {
        SectionName = sectionName;
    }
}