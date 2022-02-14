using System;
using BepInEx.Configuration;
using Memoria.FFPR.BeepInEx;
using KeyCode = UnityEngine.KeyCode;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class SavesConfiguration
{
    private const String Section = "Saves";

    private readonly ConfigEntry<Boolean> _allowPersistentStorage;

    private readonly ConfigEntry<HotkeyGroup> _quickSaveKey;
    private readonly ConfigEntry<HotkeyGroup> _quickLoadKey;

    public Boolean AllowPersistentStorage => _allowPersistentStorage.Value;

    public HotkeyGroup QuickSaveKey => _quickSaveKey.Value;
    public HotkeyGroup QuickLoadKey => _quickLoadKey.Value;

    public SavesConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

    public SavesConfiguration(ConfigFile file)
    {
        _allowPersistentStorage = file.Bind(Section, nameof(AllowPersistentStorage), true,
            $"Enables saving and loading additional gameplay data required for some features to work." +
            $"{Environment.NewLine}Enabling this option will increase the size of save files (including cloud ones)." +
            $"{Environment.NewLine}Disabling this option will result in the loss of all accumulated data the next time you save the game.");

        _quickSaveKey = file.Bind(Section, nameof(QuickSaveKey),
            defaultValue: HotkeyGroup.Create(new Hotkey(KeyCode.F5) { Alt = true }),
            description: $"Quick-save key.",
            new AcceptableHotkeyGroup(nameof(QuickSaveKey), canHold: false));

        _quickLoadKey = file.Bind(Section, nameof(QuickLoadKey),
            defaultValue: HotkeyGroup.Create(new Hotkey(KeyCode.F9) { Alt = true }),
            description: $"Quick-load key.",
            new AcceptableHotkeyGroup(nameof(QuickLoadKey), canHold: false));
    }
}