using System;
using BepInEx.Configuration;
using Memoria.FFPR.Configuration;

namespace Memoria.FFPR.Configuration.Scopes;

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
    private readonly ConfigEntry<Boolean> _exportLogNotSupportedAssets;
    private readonly ConfigEntry<Boolean> _exportLogAlreadyExportedAssets;

    private readonly ConfigEntry<Boolean> ImportEnabled;
    private readonly ConfigEntry<String> _importDirectory;
    private readonly ConfigEntry<Boolean> _importText;

    private readonly ConfigEntry<Boolean> _importTextures;
    // private readonly ConfigEntry<Boolean> _importBinary; // Cannot import :/

    private readonly ConfigEntry<Boolean> ModsEnabled;
    private readonly ConfigEntry<String> _modsDirectory;

    public AssetsConfiguration(ConfigFileProvider provider)
        : this(provider.Get(Section))
    {
    }

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
        
        _exportLogNotSupportedAssets = file.Bind(Section, nameof(ExportLogNotSupportedAssets), false,
            "Allows logging unsupported resources during export.");
        
        _exportLogAlreadyExportedAssets = file.Bind(Section, nameof(ExportLogAlreadyExportedAssets), false,
            "Allows you to log skipped, already exported resources during export.");
        
        ImportEnabled = file.Bind(Section, nameof(ImportEnabled), true,
            $"Import the supported resources from the {nameof(ImportDirectory)}.");

        _importDirectory = file.Bind(Section, nameof(ImportDirectory), "%StreamingAssets%",
            $"Directory from which the supported resources will be imported.",
            new AcceptableDirectoryPath(nameof(ImportDirectory)));

        _importText = file.Bind(Section, nameof(ImportText), true,
            "Import text resources: .txt, .csv, .json, etc.");

        _importTextures = file.Bind(Section, nameof(ImportTextures), true,
            "Import text resources: .png, .jpg, .tga");

        // _importBinary = file.Bind(Section, nameof(ImportBinary), true,
        //     "Import binary resources: .bytes, etc.");

        ModsEnabled = file.Bind(Section, nameof(ModsEnabled), true,
            $"Overwrite the supported resources from the {nameof(ModsDirectory)}.");

        _modsDirectory = file.Bind(Section, nameof(ModsDirectory), "%StreamingAssets%/Mods",
            $"Directory from which the supported resources will be updated.",
            new AcceptableDirectoryPath(nameof(ModsDirectory), create: true));
    }

    public String ExportDirectory => ExportEnabled.Value
        ? AcceptableDirectoryPath.Preprocess(_exportDirectory.Value)
        : String.Empty;

    public Boolean ExportText => _exportText.Value;
    public Boolean ExportTextures => _exportTextures.Value;
    public Boolean ExportBinary => false; // _exportBinary.Value;

    public Boolean ExportOverwrite => _exportOverwrite.Value;

    public Boolean ExportAutoDisable => _exportAutoDisable.Value;
    public Boolean ExportLogNotSupportedAssets => _exportLogNotSupportedAssets.Value;
    public Boolean ExportLogAlreadyExportedAssets => _exportLogAlreadyExportedAssets.Value;

    public String ImportDirectory => ImportEnabled.Value
        ? AcceptableDirectoryPath.Preprocess(_importDirectory.Value)
        : String.Empty;

    public Boolean ImportText => _importText.Value;
    public Boolean ImportTextures => _importTextures.Value;
    public Boolean ImportBinary => false; // _importBinary.Value;

    public String ModsDirectory => ModsEnabled.Value
        ? AcceptableDirectoryPath.Preprocess(_modsDirectory.Value)
        : String.Empty;

    public void DisableExport() => ExportEnabled.Value = false;

    public void DisableImportTextures()
    {
        if (ImportTextures)
            _importTextures.Value = false;
    }

    public void CopyFrom(AssetsConfiguration other)
    {
        ExportEnabled.Value = other.ExportEnabled.Value;
        _exportDirectory.Value = other._exportDirectory.Value;
        _exportText.Value = other._exportText.Value;
        _exportTextures.Value = other._exportTextures.Value;
        _exportOverwrite.Value = other._exportOverwrite.Value;
        _exportAutoDisable.Value = other._exportAutoDisable.Value;
        ImportEnabled.Value = other.ImportEnabled.Value;
        _importDirectory.Value = other._importDirectory.Value;
        _importText.Value = other._importText.Value;
        _importTextures.Value = other._importTextures.Value;
        ModsEnabled.Value = other.ModsEnabled.Value;
        _modsDirectory.Value = other._modsDirectory.Value;
    }
}