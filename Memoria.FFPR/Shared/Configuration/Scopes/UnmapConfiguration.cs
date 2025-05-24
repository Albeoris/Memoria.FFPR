using System;
using BepInEx.Configuration;
using Memoria.FFPR.BeepInEx;
using UnityEngine;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class UnmapConfiguration
{
    private const String Section = "Unmap";

    public ConfigEntry<Boolean> UnmapAutoDash { get; }

    public ConfigEntry<Boolean> UnmapEncountersToggle { get; }

    public UnmapConfiguration(ConfigFileProvider provider)
        : this(provider.GetAndCache(Section))
    {
    }

    public UnmapConfiguration(ConfigFile file)
    {
        UnmapAutoDash = file.Bind(Section, nameof(UnmapAutoDash), defaultValue: false,
            "Unmaps the AutoDash on/off hotkey (so it can be used for other things).");

        UnmapEncountersToggle = file.Bind(Section, nameof(UnmapEncountersToggle), defaultValue: false,
            "Unmaps the Encounters on/off hotkey (so it can be used for other things).");
    }
}