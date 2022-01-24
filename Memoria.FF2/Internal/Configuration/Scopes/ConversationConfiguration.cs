using System;
using BepInEx.Configuration;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration;
using UnityEngine;
using KeyCode = UnityEngine.KeyCode;
using Object = System.Object;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class ConversationConfiguration
{
    private const String Section = "Conversation";

    public ConfigEntry<Color> UnusedWordsColor { get; }
    public ConfigEntry<Color> UnusedKeyItemsColor { get; }
    public ConfigEntry<Color> UsedWordsColor { get; }
    public ConfigEntry<Color> UsedKeyItemsColor { get; }

    public Boolean ColorWords => UnusedWordsColor != default || UsedWordsColor != default;
    public Boolean ColorKeyItems => UnusedKeyItemsColor != default || UsedKeyItemsColor != default;

    public ConversationConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public ConversationConfiguration(ConfigFile file)
    {
        UnusedWordsColor = file.Bind(Section, nameof(UnusedWordsColor), Color.red,
            "The color that will be used to paint the secret words that you have never spoken to a specific NPC.",
            new AcceptableColor(nameof(UnusedWordsColor)));

        UnusedKeyItemsColor = file.Bind(Section, nameof(UnusedKeyItemsColor), Color.red,
            "The color that will be used to paint key items that you have not yet shown to a specific NPC.",
            new AcceptableColor(nameof(UnusedKeyItemsColor)));

        UsedWordsColor = file.Bind(Section, nameof(UsedWordsColor), default(Color),
            "The color that will be used to paint the secret words that you have already spoken to a specific NPC.",
            new AcceptableColor(nameof(UsedWordsColor)));

        UsedKeyItemsColor = file.Bind(Section, nameof(UsedKeyItemsColor), default(Color),
            "The color that will be used to paint the key items that you have already shown to a specific NPC.",
            new AcceptableColor(nameof(UsedKeyItemsColor)));
    }
}