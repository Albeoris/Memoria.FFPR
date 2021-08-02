using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Memoria.FF1.Core;

namespace Memoria.FF1.Configuration
{
    public sealed class ModConfiguration
    {
        public SpeedConfiguration Speed { get; }
        public AssetsConfiguration Assets { get; }

        public ModConfiguration()
        {
            using (var log = Logger.CreateLogSource("Memoria Config"))
            {
                try
                {
                    log.LogInfo($"Initializing {nameof(ModConfiguration)}");

                    ConfigFile file = new ConfigFile(GetConfigurationPath(), true, ownerMetadata: null);
                    Speed = new SpeedConfiguration(file);
                    Assets = new AssetsConfiguration(file);

                    log.LogInfo($"{nameof(ModConfiguration)} initialized successfully.");
                }
                catch (Exception ex)
                {
                    log.LogError($"Failed to initialize {nameof(ModConfiguration)}: {ex}");
                    throw;
                }
            }
        }
        
        private static String GetConfigurationPath()
        {
            return Utility.CombinePaths(Paths.ConfigPath, ModConstants.Id + ".cfg");
        }
    }
}