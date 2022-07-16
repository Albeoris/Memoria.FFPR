using System;
using BepInEx.Configuration;

namespace Memoria.FFPR.Configuration.Scopes;

[ConfigScope("Assets")]
public abstract partial class AssetsConfiguration
{
    [ConfigEntry($"Export the supported resources to the {nameof(ExportDirectory)}.")]
    public virtual Boolean ExportEnabled { get; set; } = false;

    [ConfigEntry($"Directory into which the supported resources will be exported.")]
    [ConfigConverter(nameof(ExportDirectoryConverter))]
    public virtual String ExportDirectory => "%StreamingAssets%";

    [ConfigEntry($"Export text resources: .txt, .csv, .json, etc.")]
    public virtual Boolean ExportText => true;

    [ConfigEntry($"Export texture resources: .png, .jpg, .tga.")]
    public virtual Boolean ExportTextures => true;

    [ConfigEntry($"Export binary resources: .bytes, etc.")]
    public virtual Boolean ExportBinary => false;

    [ConfigEntry($"Overwrites files that exist in the export directory. All your changes will be lost.")]
    public virtual Boolean ExportOverwrite => false;

    [ConfigEntry($"Automatically disable export after successful completion.")]
    public virtual Boolean ExportAutoDisable => true;

    [ConfigEntry($"Allows logging unsupported resources during export.")]
    public virtual Boolean ExportLogNotSupportedAssets => false;

    [ConfigEntry($"Allows logging already exported resources during export.")]
    public virtual Boolean ExportLogAlreadyExportedAssets => false;

    [ConfigEntry($"Import the supported resources from the {nameof(ImportDirectory)}.")]
    public virtual Boolean ImportEnabled => true;

    [ConfigEntry($"Directory from which the supported resources will be imported.")]
    [ConfigConverter(nameof(ImportDirectoryConverter))]
    public virtual String ImportDirectory => "%StreamingAssets%";

    [ConfigEntry($"Import text resources: .txt, .csv, .json, etc.")]
    public virtual Boolean ImportText => true;

    [ConfigEntry($"Import texture resources: .png, .jpg, .tga.")]
    public virtual Boolean ImportTextures { get; set; } = true;

    [ConfigEntry($"Import binary resources: .bytes, etc")]
    public virtual Boolean ImportBinary => false;

    [ConfigEntry($"Overwrite the supported resources from the {nameof(ModsDirectory)}.")]
    public virtual Boolean ModsEnabled => true;

    [ConfigEntry($"Directory from which the supported resources will be updated.")]
    [ConfigConverter(nameof(ModsDirectoryConverter))]
    [ConfigDependency(nameof(ModsEnabled), "String.Empty")]
    public virtual String ModsDirectory => "%StreamingAssets%/Mods";

    protected IAcceptableValue<String> ExportDirectoryConverter { get; } = new AcceptableDirectoryPath(nameof(ExportDirectory));
    protected IAcceptableValue<String> ImportDirectoryConverter { get; } = new AcceptableDirectoryPath(nameof(ImportDirectory));
    protected IAcceptableValue<String> ModsDirectoryConverter { get; } = new AcceptableDirectoryPath(nameof(ModsDirectory), create: true);

    public abstract void CopyFrom(AssetsConfiguration configuration);

    public String GetExportDirectoryIfEnabled() => ExportEnabled ? ExportDirectory : String.Empty;
    public String GetImportDirectoryIfEnabled() => ImportEnabled ? ImportDirectory : String.Empty;
}