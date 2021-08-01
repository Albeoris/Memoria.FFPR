using System;
using BepInEx.Logging;
using Memoria.FF1.Configuration;
using Memoria.FF1.Core;
using UnityEngine;
using Exception = System.Exception;
using IntPtr = System.IntPtr;
using Logger = BepInEx.Logging.Logger;

namespace Memoria.FF1.IL2CPP
{
    public sealed class ModComponent : MonoBehaviour
    {
        public static ModComponent Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }

        [field:NonSerialized] public ModConfiguration Config { get; private set; }
        [field:NonSerialized] public GameSpeedControl SpeedControl { get; private set; }

        public ModComponent(IntPtr ptr) : base(ptr)
        {
        }

        private Boolean _isDisabled;

        public void Awake()
        {
            Log = Logger.CreateLogSource("Memoria IL2CPP");
            Log.LogMessage($"[{nameof(ModComponent)}].{nameof(Awake)}(): Begin...");
            try
            {
                Instance = this;

                Config = new ModConfiguration();
                SpeedControl = new GameSpeedControl();

                Log.LogMessage($"[{nameof(ModComponent)}].{nameof(Awake)}(): Processed successfully.");
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(Awake)}(): {ex}");
                throw;
            }
        }

        public void OnDestroy()
        {
            Log.LogInfo($"[{nameof(ModComponent)}].{nameof(OnDestroy)}()");
        }

        private void FixedUpdate()
        {
            try
            {
                if (_isDisabled)
                    return;
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(FixedUpdate)}(): {ex}");
            }
        }

        private void Update()
        {
            try
            {
                if (_isDisabled)
                    return;
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(Update)}(): {ex}");
            }
        }
        
        private void LateUpdate()
        {
            try
            {
                if (_isDisabled)
                    return;

                // Must be called from LateUpdate to work in combat. 
                SpeedControl.Update();
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(LateUpdate)}(): {ex}");
            }
        }
    }
}