using System;
using BepInEx.Logging;
using Memoria.FF1.IL2CPP;
using UnityEngine;

namespace Memoria.FF1
{
    public sealed class SingletonInitializer
    {
        private readonly ManualLogSource _log;

        public SingletonInitializer(ManualLogSource logSource)
        {
            _log = logSource ?? throw new ArgumentNullException(nameof(logSource));
        }
        
        public void InitializeInGameSingleton()
        {
            try
            {
                String name = typeof(ModComponent).FullName;
                _log.LogInfo($"Initializing in-game singleton: {name}");

                GameObject singletonObject = new GameObject(name);
                singletonObject.hideFlags = HideFlags.HideAndDontSave;
                GameObject.DontDestroyOnLoad(singletonObject);
                
                ModComponent component = singletonObject.AddComponent<ModComponent>();
                if (component is null)
                {
                    GameObject.Destroy(singletonObject);
                    throw new Exception($"The object is missing the required component: {name}");
                }

                _log.LogInfo("In-game singleton initialized successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize in-game singleton.", ex);
            }
        }
    }
}