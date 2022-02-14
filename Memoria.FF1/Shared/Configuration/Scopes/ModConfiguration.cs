using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Core;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed partial class ModConfiguration
{
    public SavesConfiguration Saves { get; }
    public SpeedConfiguration Speed { get; }
    public EncountersConfiguration Encounters { get; }
    public BattleSystemConfiguration BattleSystem { get; }
    public BattleAtbConfiguration BattleAtb { get; }
    public FieldConfiguration Field { get; }
    public AssetsConfiguration Assets { get; }
    public GuiIndicatorsConfiguration GuiIndicators { get; }

    public ModConfiguration()
    {
        using (var log = Logger.CreateLogSource("Memoria Config"))
        {
            try
            {
                log.LogInfo($"Initializing {nameof(ModConfiguration)}");

                ConfigFileProvider provider = new();
                Saves = new SavesConfiguration(provider);
                Speed = new SpeedConfiguration(provider);
                Encounters = new EncountersConfiguration(provider);
                BattleSystem = new BattleSystemConfiguration(provider);
                BattleAtb = new BattleAtbConfiguration(provider);
                Field = new FieldConfiguration(provider);
                Assets = new AssetsConfiguration(provider);
                GuiIndicators = new GuiIndicatorsConfiguration(provider);
                InitializeGameSpecificOptions(provider);

                UpgradeLegacyConfig(log);

                log.LogInfo($"{nameof(ModConfiguration)} initialized successfully.");
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to initialize {nameof(ModConfiguration)}: {ex}");
                throw;
            }
        }
    }

    private void UpgradeLegacyConfig(ManualLogSource log)
    {
        String legacyPath = GetLegacyConfigurationPath();
        if (!File.Exists(legacyPath))
            return;
        
        log.LogInfo($"Converting the legacy config file: {legacyPath}");
        ConfigFile file = new ConfigFile(legacyPath, true, ownerMetadata: null);
        Speed.CopyFrom(new SpeedConfiguration(file));
        Encounters.CopyFrom(new EncountersConfiguration(file));
        Assets.CopyFrom(new AssetsConfiguration(file));
        File.Delete(legacyPath);
        log.LogInfo($"The old configuration file has been successfully converted to the new format and was deleted.");
    }

    private partial void InitializeGameSpecificOptions(ConfigFileProvider provider);

    private static String GetLegacyConfigurationPath()
    {
        return Utility.CombinePaths(Paths.ConfigPath, ModConstants.Id + ".cfg");
    }
}