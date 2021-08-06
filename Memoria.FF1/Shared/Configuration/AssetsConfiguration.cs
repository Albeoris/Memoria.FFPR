using System;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration
{
    public sealed class AssetsConfiguration
    {
        private const String Section = "Assets";

        private readonly ConfigEntry<Boolean> ExportEnabled;
        private readonly ConfigEntry<String> _exportDirectory;
        private readonly ConfigEntry<Boolean> _exportText;
        private readonly ConfigEntry<Boolean> _exportTextures;
        // private readonly ConfigEntry<Boolean> _exportBinary; // Cannot import :/
        private readonly ConfigEntry<Boolean> _exportOverwrite;
        private readonly ConfigEntry<Boolean> _exportAutoDisable;

        private readonly ConfigEntry<Boolean> ImportEnabled;
        private readonly ConfigEntry<String> _importDirectory;
        private readonly ConfigEntry<Boolean> _importText;
        private readonly ConfigEntry<Boolean> _importTextures;
        // private readonly ConfigEntry<Boolean> _importBinary; // Cannot import :/

        public AssetsConfiguration(ConfigFile file)
        {
            ExportEnabled = file.Bind(Section, nameof(ExportEnabled), false,
                $"Export the supported resources to the {nameof(ExportDirectory)}.");

            _exportDirectory = file.Bind(Section, nameof(ExportDirectory), "%StreamingAssets%",
                $"Directory into which the supported resources will be exported.",
                new AcceptableDirectoryPath(nameof(ExportDirectory)));

            _exportText = file.Bind(Section, nameof(ExportText), true,
                "Export text resources: .txt, .csv, .json, etc.");
            
            _exportTextures = file.Bind(Section, nameof(ExportTextures), true,
                "Export texture resources: .png, .jpg, .tga");
            
            // _exportBinary = file.Bind(Section, nameof(ExportBinary), true,
            //     "Export binary resources: .bytes, etc.");

            _exportOverwrite = file.Bind(Section, nameof(ExportOverwrite), false,
                "Overwrites files that exist in the export directory. All your changes will be lost.");
            
            _exportAutoDisable = file.Bind(Section, nameof(ExportAutoDisable), true,
                "Automatically disable export after successful completion.");
            
            ImportEnabled = file.Bind(Section, nameof(ImportEnabled), true,
                $"Import the supported resources from the {nameof(ImportDirectory)}.");

            _importDirectory = file.Bind(Section, nameof(ImportDirectory), "%StreamingAssets%",
                $"Directory from which the supported resources will be imported.",
                new AcceptableDirectoryPath(nameof(ImportDirectory)));
            
            _importText = file.Bind(Section, nameof(ImportText), true,
                "Import text resources: .txt, .csv, .json, etc.");
            
            _importTextures = file.Bind(Section, nameof(ImportTextures), false,
                "Import text resources: .png, .jpg, .tga");
            
            // _importBinary = file.Bind(Section, nameof(ImportBinary), true,
            //     "Import binary resources: .bytes, etc.");
        }

        public String ExportDirectory => ExportEnabled.Value
            ? AcceptableDirectoryPath.Preprocess(_exportDirectory.Value)
            : String.Empty;

        public Boolean ExportText => _exportText.Value;
        public Boolean ExportTextures => _exportTextures.Value;
        public Boolean ExportBinary => false; // _exportBinary.Value;
        
        public Boolean ExportOverwrite => _exportOverwrite.Value;

        public Boolean ExportAutoDisable => _exportAutoDisable.Value;

        public String ImportDirectory => ImportEnabled.Value
            ? AcceptableDirectoryPath.Preprocess(_importDirectory.Value)
            : String.Empty;
        
        public Boolean ImportText => _importText.Value;
        public Boolean ImportTextures => _importTextures.Value;
        public Boolean ImportBinary => false; // _importBinary.Value;

        public void DisableExport() => ExportEnabled.Value = false;
    }
}