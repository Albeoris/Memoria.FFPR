using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Linq;
using Last.Data.Master;
using Last.Defaine;
using Last.Management;
using Last.Systems;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.FF6.Internal;
using Memoria.FFPR.IL2CPP;
using Serial.FF6.Management;
using Newtonsoft.Json;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(MagicStoneData), nameof(MagicStoneData.GetAllIds))]
public static class MagicStoneData_Ctor
{
    private static Boolean _patched = false;
    
    // ReSharper disable InconsistentNaming
    public static void Prefix(MagicStoneData __instance)
    {
        try
        {
            if (_patched)
                return;
            _patched = true;
            
            TryExport(__instance);
            TryImport(__instance);
            TryMod(__instance);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void TryImport(MagicStoneData instance)
    {
        try
        {
            String importPath = TryGetImportPath();
            if (importPath == String.Empty)
                return;
            
            var abilities = instance.AbilityList;
            var types = instance.ParamterTypeList;
            var values = instance.ParameterValueList;
            var messages = instance.magicStoneBonusMessageIdPairs;

            abilities.Clear();
            types.Clear();
            values.Clear();
            messages.Clear();

            ApplyMod(instance, importPath);
            
            String shortPath = ApplicationPathConverter.ReturnPlaceholders(importPath);
            ModComponent.Log.LogInfo($"[{MagiciteFileName}] {shortPath} imported successfully.");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex, $"[{MagiciteFileName}] Failed to import {MagiciteFileName}.");
        }
    }

    private static void TryExport(MagicStoneData instance)
    {
        String exportPath = null;
        try
        {
            exportPath = TryGetExportPath();
            if (exportPath == String.Empty)
                return;

            Dictionary<Int32, Int32> abilities = instance.AbilityList.ToManaged();;
            Dictionary<Int32, ParameterType> parameterTypes = instance.ParamterTypeList.ToManaged();;
            Dictionary<Int32, Int32> parameterValues = instance.ParameterValueList.ToManaged();;
            Dictionary<Int32, String> messages = instance.magicStoneBonusMessageIdPairs.ToManaged();;

            using (StreamWriter output = File.CreateText(exportPath))
            {
                WriteHeader(output);

                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;

                JsonTextWriter json = new JsonTextWriter(output);
                json.Formatting = Formatting.Indented;
                
                json.WriteStartArray();
                {
                    foreach (Int32 id in instance.GetAllIds().ToArray())
                    {
                        if (!abilities.TryGetValue(id, out Int32 abilityId))
                            throw new Exception($"Cannot get ability of magicite {id}");

                        parameterTypes.TryGetValue(id, out ParameterType type);
                        parameterValues.TryGetValue(id, out Int32 value);
                        messages.TryGetValue(id, out String messageKey);

                        if (messageKey is not null && type > ParameterType.Non)
                            messageKey = $"{messageKey} -> {value}";

                        String abilityName = ContentUtitlity.GetAbilityName(abilityId);
                        String magiciteDescription = messageKey is null ? null : MessageManager.Instance.GetMessage(messageKey);
                        MagiciteInfo info = new MagiciteInfo(id, abilityId, type, value, messageKey, magiciteDescription);

                        json.WriteComment(abilityName);
                        if (!String.IsNullOrWhiteSpace(magiciteDescription))
                            json.WriteComment(magiciteDescription);
                        serializer.Serialize(json, info);
                    }
                }
                json.WriteEndArray();
                json.Flush();
                WriteFooter(output);
            }
            
            if (ModComponent.Instance.Config.Assets.ExportAutoDisable)
                ModComponent.Instance.Config.Magicite.ExportMagicite = false;
            
            String shortPath = ApplicationPathConverter.ReturnPlaceholders(exportPath);
            ModComponent.Log.LogInfo($"[{MagiciteFileName}] {shortPath} exported successfully.");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex, $"[{MagiciteFileName}] Failed to export {MagiciteFileName}.");

            if (exportPath != null && File.Exists(exportPath))
                File.Delete(exportPath);
        }
    }

    private static void TryMod(MagicStoneData instance)
    {
        try
        {
            Int32 count = 0;
            IReadOnlyList<String> modFiles = FindModFiles();
            foreach (String modPath in modFiles)
            {
                try
                {
                    ApplyMod(instance, modPath);
                    count++;
                }
                catch (Exception ex)
                {
                    String shortPath = ApplicationPathConverter.ReturnPlaceholders(modPath);
                    ModComponent.Log.LogException(ex, $"[{MagiciteFileName}] Failed to apply mod {shortPath}.");
                }
            }

            if (count > 0)
                ModComponent.Log.LogInfo($"[{MagiciteFileName}] {count} applied successfully.");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex, $"[{MagiciteFileName}] Failed to modify.");
        }
    }

    private static void ApplyMod(MagicStoneData instance, String importPath)
    {
        using (StreamReader input = File.OpenText(importPath))
        {
            JsonSerializer serializer = new JsonSerializer();
            JsonTextReader json = new JsonTextReader(input);

            MagiciteInfo[] objects = serializer.Deserialize<MagiciteInfo[]>(json);
            if (objects is null)
                throw new Exception("Failed to deserialize magicites data.");

            var abilities = instance.AbilityList;
            var types = instance.ParamterTypeList;
            var values = instance.ParameterValueList;
            var messages = instance.magicStoneBonusMessageIdPairs;

            MasterManager master = Il2CppSystem.SingletonMonoBehaviour<MasterManager>.Instance;
            MessageManager messageManager = MessageManager.Instance;

            foreach (MagiciteInfo item in objects)
            {
                if (master.GetData<Ability>(item.AbilityId) is null)
                    ModComponent.Log.LogWarning($"[{MagiciteFileName}] Magicite {item.Id} references to not existing ability {item.AbilityId}.");

                abilities[item.Id] = item.AbilityId;

                if (item.ParameterType > ParameterType.Non)
                {
                    types[item.Id] = item.ParameterType;
                    values[item.Id] = item.ParameterValue;
                }

                if (item.DescriptionKey != null)
                {
                    if (messageManager.GetMessage(item.DescriptionKey) == String.Empty)
                        ModComponent.Log.LogWarning($"[{MagiciteFileName}] Magicite {item.Id} references to not existing description {item.DescriptionKey}.");

                    messages[item.Id] = item.DescriptionKey;
                }
            }
        }
    }

    private static void WriteHeader(StreamWriter output)
    {
        output.WriteLine("/*");
        output.WriteLine("This file contains the description of Magicites.");
        output.WriteLine("They can be imported back to the game using the ImportMagicite or ModsEnabled options.");
        output.WriteLine("Look at the end of the file for all available abilities and parameter types.");
        output.WriteLine("*/");
        output.WriteLine();
    }

    private static void WriteFooter(StreamWriter output)
    {
        output.WriteLine();
        output.WriteLine();
        output.WriteLine("/*");

        output.WriteLine("Available parameter types:");
        foreach (String type in Enum.GetNames(typeof(ParameterType)))
            output.WriteLine($"\t{type}");
        output.WriteLine();

        output.WriteLine("Available abilities:");

        List<(Int32 Id, String Name, String Description)> list = new();
        Dictionary<Int32, Ability> allAbilities = MasterManager.Instance.GetList<Ability>().ToManaged();
        foreach (var pairs in allAbilities.OrderBy(p => p.Key))
        {
            Int32 id = pairs.Key;
            Ability ab = pairs.Value;
            String abName = ContentUtitlity.GetAbilityName(ab);
            String abDesc = ContentUtitlity.GetAbilityDescription(ab);

            if (String.IsNullOrWhiteSpace(abName))
                continue;

            if (String.IsNullOrWhiteSpace(abDesc))
                output.WriteLine($"\t{id:d4} - {abName}");
            else
                output.WriteLine($"\t{id:d4} - {abName} ({abDesc})");
        }

        output.WriteLine("*/");
    }

    private static String TryGetExportPath()
    {
        MagiciteConfiguration magicite = ModComponent.Instance.Config.Magicite;

        String exportDirectory =  ModComponent.Instance.Config.Assets.ExportDirectory;
        if (exportDirectory == String.Empty)
            return String.Empty;

        if (!magicite.ExportMagicite)
            return String.Empty;

        Boolean overwrite = ModComponent.Instance.Config.Assets.ExportOverwrite;
        String filePath = GetMagiciteFilePath(exportDirectory);
        if (ResourceExporter.HandleExistingFile("ExportMagicite", MagiciteFileName, filePath, overwrite))
            return String.Empty;

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        return filePath;
    }

    private static String TryGetImportPath()
    {
        MagiciteConfiguration magicite = ModComponent.Instance.Config.Magicite;

        String importDirectory = ModComponent.Instance.Config.Assets.ImportDirectory;
        if (importDirectory == String.Empty)
            return String.Empty;

        if (!magicite.ImportMagicite)
            return String.Empty;

        String filePath = GetMagiciteFilePath(importDirectory);
        return File.Exists(filePath)
            ? filePath
            : String.Empty;
    }
    
    private static IReadOnlyList<String> FindModFiles()
    {
        String assetAddress = $"{MagiciteDirectoryName}/{MagiciteFileName}";
        return ModComponent.Instance.ModFiles.FindAll(assetAddress);
    }

    private const String MagiciteDirectoryName = "Memoria";
    private const String MagiciteFileName = "Magicite.json";

    private static String GetMagiciteFilePath(String rootDirectory)
    {
        String directory = Path.Combine(rootDirectory, MagiciteDirectoryName);
        return Path.Combine(directory, "Magicite.json");
    }
}