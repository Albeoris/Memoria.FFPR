using System;
using BepInEx.Configuration;
using KeyCode = UnityEngine.KeyCode;

namespace Memoria.FFPR.Configuration
{
    public sealed class EncountersConfiguration
    {
        private const String Section = "Encounters";

        public ConfigEntry<KeyCode> ToggleKey { get; }
        public ConfigEntry<KeyCode> HoldKey { get; }
        public ConfigEntry<String> ToggleAction { get; }
        public ConfigEntry<String> HoldAction { get; }

        public EncountersConfiguration(ConfigFile file)
        {
            ToggleKey = file.Bind(Section, nameof(ToggleKey), KeyCode.F2,
                $"Disable/Enable encounters toggle key.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");

            HoldKey = file.Bind(Section, nameof(HoldKey), KeyCode.None,
                $"Disable encounters hold key.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");

            ToggleAction = file.Bind(Section, nameof(ToggleAction), "None",
                $"Disable/Enable encounters action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            HoldAction = file.Bind(Section, nameof(HoldAction), "None",
                $"Disable encounters hold action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));
        }
    }
}