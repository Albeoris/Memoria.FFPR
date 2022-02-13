using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Memoria.FFPR.Configuration;
using KeyCode = UnityEngine.KeyCode;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class EncountersConfiguration
{
    private const String Section = "Encounters";

    public ConfigEntry<HotkeyGroup> Key { get; }

    public EncountersConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public EncountersConfiguration(ConfigFile file)
    {
        Key = file.Bind(Section, nameof(Key),
            defaultValue: HotkeyGroup.Create(new[]
            {
                new Hotkey(KeyCode.F2),
            }),
            description: $"Disable encounters key.",
            new AcceptableHotkeyGroup(nameof(Key)));

        UpdateOldConfig(file);
    }

    private void UpdateOldConfig(ConfigFile file)
    {
        List<Hotkey> list = new List<Hotkey>();

        {
            ConfigEntry<Hotkey> toggleKey = file.Bind(Section, "ToggleKey", Hotkey.None,
                $"Disable/Enable encounters toggle key.",
                new AcceptableHotkey("ToggleKey"));

            Hotkey key = toggleKey.Value;
            if (key.Key != KeyCode.None)
                list.Add(key);

            file.Remove(toggleKey.Definition);
        }

        {
            ConfigEntry<Hotkey> holdKey = file.Bind(Section, "HoldKey", Hotkey.None,
                $"Disable encounters hold key.",
                new AcceptableHotkey("HoldKey"));

            Hotkey key = holdKey.Value;
            if (key.Key != KeyCode.None)
            {
                key.MustHeld = true;
                list.Add(key);
            }

            file.Remove(holdKey.Definition);
        }

        {
            ConfigEntry<String> toggleAction = file.Bind(Section, "ToggleAction", "None",
                $"Disable/Enable encounters action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            String action = toggleAction.Value;
            if (action != "None")
                list.Add(new Hotkey(action));

            file.Remove(toggleAction.Definition);
        }

        {
            ConfigEntry<String> holdAction = file.Bind(Section, "HoldAction", "None",
                $"Disable encounters hold action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            String action = holdAction.Value;
            if (action != "None")
                list.Add(new Hotkey(action) { MustHeld = true });

            file.Remove(holdAction.Definition);
        }

        if (list.Count > 0)
            Key.Value = HotkeyGroup.Create(list);
    }

    public void CopyFrom(EncountersConfiguration other)
    {
        Key.Value = other.Key.Value;
    }
}