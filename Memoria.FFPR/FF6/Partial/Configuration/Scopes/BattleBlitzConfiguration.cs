using System;

namespace Memoria.FFPR.Configuration.Scopes;

[ConfigScope("Battle.Blitz")]
public abstract partial class BattleBlitzConfiguration
{
    [ConfigEntry($"Replaces the key sequence required to lunch a Blitz Ability with a Confirmation button.")]
    public virtual Boolean EasyInput { get; set; } = false;
    
    public abstract void CopyFrom(BattleBlitzConfiguration configuration);
}