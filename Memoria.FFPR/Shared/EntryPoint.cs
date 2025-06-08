using System;
using System.Reflection;
using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Memoria.FFPR.Core;

namespace Memoria.FFPR;

[BepInPlugin(ModConstants.Id, "Memoria FF PR", "1.0.0.0")]
public class EntryPoint : BasePlugin
{
    public override void Load()
    {
        try
        {
            Log.LogMessage("Initializing...");

            TypeRegister typeRegister = new(Log);
            typeRegister.RegisterRequiredTypes();
            typeRegister.RegisterAssemblyTypes();

            SingletonInitializer singletonInitializer = new(Log);
            singletonInitializer.InitializeInGameSingleton();

            PatchMethods();
            Log.LogMessage("The mod has been successfully initialized.");
        }
        catch (Exception ex)
        {
            Log.LogError($"Failed to initialize the mod: {ex}");
            throw;
        }
    }

    private void PatchMethods()
    {
        try
        {
            Log.LogInfo("[Harmony] Patching methods...");
            Harmony harmony = new Harmony(ModConstants.Id);
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (var type in AccessTools.GetTypesFromAssembly(assembly))
            {
                PatchClassProcessor processor = harmony.CreateClassProcessor(type);
                if (processor.Patch()?.Count > 0)
                    Log.LogInfo($"[Harmony] {type.Name} successfully applied.");
            }
            // AccessTools.GetTypesFromAssembly(assembly).Do(type => harmony.CreateClassProcessor(type).Patch());
            // //public void PatchAll(Assembly assembly) => ((IEnumerable<Type>) AccessTools.GetTypesFromAssembly(assembly)).Do<Type>((Action<Type>) (type => this.CreateClassProcessor(type).Patch()));
            // harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to patch methods.", ex);
        }
    }
}