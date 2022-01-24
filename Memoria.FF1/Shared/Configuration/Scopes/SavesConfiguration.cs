using System;
using BepInEx.Configuration;
using KeyCode = UnityEngine.KeyCode;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed class SavesConfiguration
{
    private const String Section = "Saves";

    private readonly ConfigEntry<Boolean> _allowPersistentStorage;
    
    private readonly ConfigEntry<KeyCode> _quickSaveKey;
    private readonly ConfigEntry<KeyCode> _quickLoadKey;
    private readonly ConfigEntry<String> _quickSaveAction;
    private readonly ConfigEntry<String> _quickLoadAction;
    
    public Boolean AllowPersistentStorage => _allowPersistentStorage.Value;
    
    public KeyCode QuickSaveKey => _quickSaveKey.Value;
    public KeyCode QuickLoadKey => _quickLoadKey.Value;
    public String QuickSaveAction => _quickSaveAction.Value;
    public String QuickLoadAction => _quickLoadAction.Value;

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
        
        _quickSaveKey = file.Bind(Section, nameof(QuickSaveKey), KeyCode.F5,
            $"Disable/Enable quick-save key.");
        
        _quickLoadKey = file.Bind(Section, nameof(QuickLoadKey), KeyCode.F9,
            $"Disable/Enable quick-load key.");
        
        _quickSaveAction = file.Bind(Section, nameof(QuickSaveAction), "None",
            $"Disable/Enable quick-save action.",
            new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));
        
        _quickLoadAction = file.Bind(Section, nameof(QuickLoadAction), "None",
            $"Disable/Enable quick-load action.",
            new AcceptableValueList<String>("None", "Enter", "Cancel", "Shortcut", "Menu", "Up", "Down", "Left", "Right", "SwitchLeft", "SwitchRight", "PageUp", "PageDown", "Start"));
    }
}