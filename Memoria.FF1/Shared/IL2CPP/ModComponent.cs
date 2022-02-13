using System;
using BepInEx.Logging;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Core;
using Memoria.FFPR.Mods;
using UnityEngine;
using Exception = System.Exception;
using IntPtr = System.IntPtr;
using Logger = BepInEx.Logging.Logger;
using Object = System.Object;

namespace Memoria.FFPR.IL2CPP;

public sealed class ModComponent : MonoBehaviour
{
    public static ModComponent Instance { get; private set; }
    public static ManualLogSource Log { get; private set; }

    [field: NonSerialized] public ModConfiguration Config { get; private set; }
    [field: NonSerialized] public GameSaveLoadControl SaveLoadControl { get; private set; }
    [field: NonSerialized] public GameSpeedControl SpeedControl { get; private set; }
    [field: NonSerialized] public GameEncountersControl EncountersControl { get; private set; }
    [field: NonSerialized] public GameEntitiesControl FieldControl { get; private set; }
    [field: NonSerialized] public ModFileResolver ModFiles { get; private set; }
    [field: NonSerialized] public ScreenDrawer Drawer { get; private set; }

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
            SaveLoadControl = new GameSaveLoadControl();
            SpeedControl = new GameSpeedControl();
            EncountersControl = new GameEncountersControl();
            FieldControl = new GameEntitiesControl();
            ModFiles = new ModFileResolver();

            Drawer = gameObject.AddComponent<ScreenDrawer>();
            gameObject.AddComponent<ResourceExporter>();

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

            EncountersControl.Update();
            FieldControl.TryUpdate();
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

            // Must be called from LateUpdate to work with actual data.
            SaveLoadControl.Update();

            // Must be called from LateUpdate to work in combat. 
            SpeedControl.TryUpdate();
        }
        catch (Exception ex)
        {
            _isDisabled = true;
            Log.LogError($"[{nameof(ModComponent)}].{nameof(LateUpdate)}(): {ex}");
        }
    }
}