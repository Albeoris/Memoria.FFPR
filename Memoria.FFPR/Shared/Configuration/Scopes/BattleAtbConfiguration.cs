using System;
using BepInEx.Configuration;
using Memoria.FFPR.BeepInEx;
using UnityEngine;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class BattleAtbConfiguration
{
    private const String Section = "Battle.ATB";

    public ConfigEntry<Boolean> TurnBased { get; }
    public ConfigEntry<HotkeyGroup> NextTurnKey { get; }

    public BattleAtbConfiguration(ConfigFileProvider provider)
        : this(provider.GetAndCache(Section))
    {
    }

    public BattleAtbConfiguration(ConfigFile file)
    {
        TurnBased = file.Bind(Section, nameof(TurnBased), defaultValue: false,
            "Changes waiting mode to turn-based mode. ATB will not increase while the command window is open.");

        NextTurnKey = file.Bind(Section, nameof(NextTurnKey),
            defaultValue: HotkeyGroup.Create(new Hotkey(KeyCode.F)),
            description: $"Allows the ATB increase until the next character's turn.",
            new AcceptableHotkeyGroup(nameof(NextTurnKey), canHold: false));
    }
}