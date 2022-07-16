using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using Last.Entity.Field;
using Memoria.FFPR.BeepInEx;
using UnityEngine;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class FieldConfiguration
{
    private const String Section = "Highlighting";

    public ConfigEntry<HotkeyGroup> HighlightingKey { get; }
    public ConfigEntry<IReadOnlyList<FieldInteractiveIcon.InteractiveIconType>> HighlightingIcons { get; }
    public ConfigEntry<Boolean> HighlightHiddenPassages { get; }

    public FieldConfiguration(ConfigFileProvider provider)
        : this(provider.GetAndCache($"Field.{Section}"))
    {
    }

    public FieldConfiguration(ConfigFile file)
    {
        HighlightingKey = file.Bind(Section, nameof(HighlightingKey),
            defaultValue: HotkeyGroup.Create(new[]
            {
                new Hotkey(KeyCode.CapsLock),
                new Hotkey(KeyCode.CapsLock) { MustHeld = true }
            }),
            description: $"Enables or disables highlighting of interactive objects.",
            new AcceptableHotkeyGroup(nameof(HighlightingKey), canHold: true));

        HighlightingIcons = file.Bind(Section, nameof(HighlightingIcons),
            defaultValue: (IReadOnlyList<FieldInteractiveIcon.InteractiveIconType>)new[]
            {
                FieldInteractiveIcon.InteractiveIconType.GetOn,
                FieldInteractiveIcon.InteractiveIconType.TreasureBox
            },
            description: $"Allowed types of highlighted icons.",
            new AcceptableEnumList<FieldInteractiveIcon.InteractiveIconType>(nameof(HighlightingIcons)));

        HighlightHiddenPassages = file.Bind(Section, nameof(HighlightHiddenPassages),
            defaultValue: false,
            description: "Pressing the key highlights hidden passages.");

        
        //InteractiveIconType
    }

    public void CopyFrom(FieldConfiguration other)
    {
        HighlightingKey.Value = other.HighlightingKey.Value;
        HighlightingIcons.Value = other.HighlightingIcons.Value;
        HighlightHiddenPassages.Value = other.HighlightHiddenPassages.Value;
    }
}