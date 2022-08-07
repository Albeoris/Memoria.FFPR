using System;
using HarmonyLib;
using Last.Entity.Field;
using Last.Map;
using Memoria.FF2.Internal.Core;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF2.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(FieldNonPlayerController), nameof(FieldNonPlayerController.OnTakeAciton))]
public static class FieldNonPlayerController_OnTakeAction
{
    public static void Prefix(FieldNonPlayerController __instance)
    {
        try
        {
            ChangeLastInteractionEntity(__instance);
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }

    private static void ChangeLastInteractionEntity(FieldNonPlayerController controller)
    {
        if (controller is null)
            throw new ArgumentNullException(nameof(controller));

        MapManager manager = MapLoadProcessor.Instance.mapManager;
        MapModel currentMap = manager.CurrentMapModel;

        FieldNonPlayer entity = controller.NonPlayerEntity;
        PropertyEntity property = entity.Property;

        FieldEntityId desc = new(currentMap.mapId, property.EntityId);
        ConversationManager.ChangeLastInteractionEntity(desc);
    }
}