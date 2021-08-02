using System;
using BepInEx.Configuration;
using KeyCode = UnityEngine.KeyCode;

namespace Memoria.FFPR.Configuration
{
    public sealed class SpeedConfiguration
    {
        private const String Section = "Speed";

        public ConfigEntry<KeyCode> ToggleKey { get; }
        public ConfigEntry<KeyCode> HoldKey { get; }
        public ConfigEntry<String> ToggleAction { get; }
        public ConfigEntry<String> HoldAction { get; }
        public ConfigEntry<Single> ToggleFactor { get; }
        public ConfigEntry<Single> HoldFactor { get; }

        public SpeedConfiguration(ConfigFile file)
        {
            ToggleKey = file.Bind(Section, nameof(ToggleKey), KeyCode.F1,
                $"Speed up toggle key.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");

            HoldKey = file.Bind(Section, nameof(HoldKey), KeyCode.None,
                $"Speed up hold key.{Environment.NewLine}https://docs.unity3d.com/ScriptReference/KeyCode.html");

            ToggleAction = file.Bind(Section, nameof(ToggleAction), "None",
                $"Speed up toggle action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            HoldAction = file.Bind(Section, nameof(HoldAction), "None",
                $"Speed up hold action.",
                new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));

            ToggleFactor = file.Bind(Section, nameof(ToggleFactor), 3.0f,
                "Speed up toggle factor.",
                0.01f, 10.0f);

            HoldFactor = file.Bind(Section, nameof(HoldFactor), 5.0f,
                "Speed up hold factor.",
                0.01f, 10.0f);
        }
    }
}