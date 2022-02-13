using System;
using BepInEx.Configuration;
using Memoria.FFPR.BeepInEx;
using UnityEngine;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class GuiIndicatorsConfiguration
{
    private const String Section = "Gui.Indicators";

    public ConfigEntry<Boolean> Enabled { get; }
    public ConfigEntry<Int32> TextSize { get; }
    public ConfigEntry<Color> TextColor { get; }
    public ConfigEntry<TextAnchor> TextAlignment { get; }

    public GuiIndicatorsConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public GuiIndicatorsConfiguration(ConfigFile file)
    {
        Enabled = file.Bind(Section, nameof(Enabled), defaultValue: true,
            "Enables the display of indicators of various modes in the corner of the screen (acceleration, disabling random battles, displaying hidden objects, etc.)");

        TextSize = file.Bind(Section, nameof(TextSize), defaultValue: 28,
            "The font size that will be used to display the indicators.");
        
        TextColor = file.Bind(Section, nameof(TextColor), Color.white,
            "The color that will be used to display the indicators.",
            new AcceptableColor(nameof(TextColor)));
        
        TextAlignment = file.Bind(Section, nameof(TextAlignment), defaultValue: TextAnchor.UpperLeft,
            "The position on the screen where the indicators will be displayed.");
    }
}