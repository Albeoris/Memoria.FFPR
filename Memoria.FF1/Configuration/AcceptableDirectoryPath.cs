using System;
using System.IO;
using BepInEx.Configuration;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;
using Object = System.Object;

namespace Memoria.FF1.Configuration
{
    public sealed class AcceptableDirectoryPath : AcceptableValueBase
    {
        private readonly String _optionName;

        public AcceptableDirectoryPath(String optionName) : base(typeof(String))
        {
            _optionName = optionName;
        }

        public override Object Clamp(Object value)
        {
            if (IsValid(value, out var error))
                return value;

            using (var log = Logger.CreateLogSource("Memoria Config"))
                log.LogError($"[{_optionName}] Provided path [{value}] is invalid and will be replaced with an empty string. Reason: {error}");

            return String.Empty;
        }

        public override Boolean IsValid(Object value)
        {
            return IsValid(value, out _);
        }

        public Boolean IsValid(Object value, out String error)
        {
            try
            {
                if (value is null)
                {
                    error = "Value is null.";
                    return false;
                }

                if (value is String str)
                {
                    if (str == String.Empty)
                    {
                        error = null;
                        return true;
                    }

                    String replaced = Preprocess(str);
                    Boolean isExists = Directory.Exists(replaced);
                    error = isExists ? null : $"Directory [{replaced}] does not exist.";
                    return isExists;
                }

                error = $"Invalid value type: {value.GetType().FullName}";
                return false;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
        }

        public override String ToDescriptionString()
        {
            return "# Acceptable values: absolute or relative path to the EXISTING directory. Placeholders: %StreamingAssets%, %DataPath%, %PersistentDataPath%, %TemporaryCachePath%";
        }

        public static String Preprocess(String path)
        {
            if (path is null)
                return null;

            if (path.IndexOf('%') < 0)
                return Path.GetFullPath(path);

            String result = path.Replace("%StreamingAssets%", Application.streamingAssetsPath)
                .Replace("%DataPath%", Application.dataPath)
                .Replace("%PersistentDataPath%", Application.persistentDataPath)
                .Replace("%TemporaryCachePath%", Application.temporaryCachePath);

            return Path.GetFullPath(result);
        }
    }
}