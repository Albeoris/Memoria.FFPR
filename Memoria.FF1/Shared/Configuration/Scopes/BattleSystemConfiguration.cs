using System;
using BepInEx.Configuration;
using Last.Management;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class BattleSystemConfiguration
{
    private const String Section = "Battle.System";

    public ConfigEntry<Boolean> ChangeBattleType { get; }
    public ConfigEntry<BattleType> BattleType { get; }

    public BattleSystemConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public BattleSystemConfiguration(ConfigFile file)
    {
        ChangeBattleType = file.Bind(Section, nameof(ChangeBattleType), defaultValue: false,
            "Allows to override the combat system using the settings below. Some combat mechanics may have changed.");

        BattleType = file.Bind(Section, nameof(BattleType), defaultValue: Last.Management.BattleType.Command,
            $"Overrides the combat system if {nameof(ChangeBattleType)} is true.");
    }
}