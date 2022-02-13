using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Memoria.FFPR.Configuration;
using KeyCode = UnityEngine.KeyCode;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class SpeedConfiguration
{
    private const String Section = "Speed";

    public ConfigEntry<HotkeyGroup> Key { get; }
    public ConfigEntry<Single> ToggleFactor { get; }
    public ConfigEntry<Single> HoldFactor { get; }

    public SpeedConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public SpeedConfiguration(ConfigFile file)
    {
        Key = file.Bind(Section, nameof(Key),
            defaultValue: HotkeyGroup.Create(new[]
            {
                new Hotkey(KeyCode.F1),
                new Hotkey(KeyCode.F1) { MustHeld = true }
            }),
            description: $"Speed up key.",
            new AcceptableHotkeyGroup(nameof(Key)));

        UpdateOldConfig(file);

        ToggleFactor = file.Bind(Section, nameof(ToggleFactor), 3.0f,
            "Speed up toggle factor.",
            0.01f, 10.0f);

        HoldFactor = file.Bind(Section, nameof(HoldFactor), 5.0f,
            "Speed up hold factor.",
            0.01f, 10.0f);
    }

    private void UpdateOldConfig(ConfigFile file)
    {
        List<Hotkey> list = new List<Hotkey>();
        
        {
            ConfigEntry<Hotkey> toggleKey = file.Bind(Section, "ToggleKey", Hotkey.None,
                $"Speed up toggle key.",
                new AcceptableHotkey("ToggleKey"));

            Hotkey key = toggleKey.Value;
            if (key.Key != KeyCode.None)
                list.Add(key);

            file.Remove(toggleKey.Definition);
        }

        {
            ConfigEntry<Hotkey> holdKey = file.Bind(Section, "HoldKey", Hotkey.None,
                $"Speed up hold key.",
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
                $"Speed up toggle action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            String action = toggleAction.Value;
            if (action != "None")
                list.Add(new Hotkey(action));

            file.Remove(toggleAction.Definition);
        }

        {
            ConfigEntry<String> holdAction = file.Bind(Section, "HoldAction", "None",
                $"Speed up hold action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            String action = holdAction.Value;
            if (action != "None")
                list.Add(new Hotkey(action) { MustHeld = true });

            file.Remove(holdAction.Definition);
        }

        if (list.Count > 0)
            Key.Value = HotkeyGroup.Create(list);
    }

    public void CopyFrom(SpeedConfiguration other)
    {
        Key.Value = other.Key.Value;
        ToggleFactor.Value = other.ToggleFactor.Value;
        HoldFactor.Value = other.HoldFactor.Value;
    }
}