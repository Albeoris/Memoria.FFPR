using System;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;
using Newtonsoft.Json.Linq;

namespace Memoria.FFPR;

public sealed partial class SaveManager
{
    public static String SaveData(String json)
    {
        try
        {
            if (!ModComponent.Instance.Config.Saves.AllowPersistentStorage)
            {
                ModComponent.Log.LogWarning($"Saving persistent data is disabled according to the settings: [Saves].AllowPersistentStorage] = false");
                return json;
            }

            JObject root = JObject.Parse(json);
            SaveData(root);
            
            String result = root.ToString();
            
            // Spam
            // ModComponent.Log.LogInfo($"Permanent data successfully saved.");
            
            return result;
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex, $"Failed to save persistent data.");
            return json;
        }
    }

    public static void LoadData(String json)
    {
        try
        {
            if (!ModComponent.Instance.Config.Saves.AllowPersistentStorage)
            {
                ModComponent.Log.LogWarning($"Loading persistent data is disabled according to the settings: [Saves].AllowPersistentStorage] = false");
                ResetData();
                return;
            }

            JObject root = JObject.Parse(json);
            LoadData(root);
            ModComponent.Log.LogInfo($"Permanent data successfully loaded.");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex, $"Failed to load persistent data.");
        }
    }

    private static partial void SaveData(JObject root);
    private static partial void LoadData(JObject root);
    private static partial void ResetData();
}