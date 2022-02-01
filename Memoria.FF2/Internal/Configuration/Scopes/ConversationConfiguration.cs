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
    public ConfigEntry<Color> UsedWordsColor { get; }

    public Boolean ColorWords => UnusedWordsColor != default || UsedWordsColor != default;

    public ConversationConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public ConversationConfiguration(ConfigFile file)
    {
        UnusedWordsColor = file.Bind(Section, nameof(UnusedWordsColor), Color.red,
            "The color that will be used to paint the secret words that you have never spoken to a specific NPC.",
            new AcceptableColor(nameof(UnusedWordsColor)));

        UsedWordsColor = file.Bind(Section, nameof(UsedWordsColor), default(Color),
            "The color that will be used to paint the secret words that you have already spoken to a specific NPC.",
            new AcceptableColor(nameof(UsedWordsColor)));
    }
}