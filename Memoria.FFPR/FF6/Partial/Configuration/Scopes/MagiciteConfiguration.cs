using System;

namespace Memoria.FFPR.Configuration.Scopes;

[ConfigScope("Assets")]
public abstract partial class MagiciteConfiguration
{
    [ConfigEntry($"Export magicite level-up bonuses. Ignores {nameof(AssetsConfiguration.ExportEnabled)}.")]
    public virtual Boolean ExportMagicite { get; set; } = true;
    
    [ConfigEntry($"Import magicite level-up bonuses. Ignores {nameof(AssetsConfiguration.ImportEnabled)}.")]
    public virtual Boolean ImportMagicite { get; set; } = true;
    
    public abstract void CopyFrom(MagiciteConfiguration configuration);
}