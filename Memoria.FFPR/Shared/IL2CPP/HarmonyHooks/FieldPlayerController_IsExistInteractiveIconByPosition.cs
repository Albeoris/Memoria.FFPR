using System;
using HarmonyLib;
using Last.Entity.Field;
using Last.Map;
using UnityEngine;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(FieldPlayerController), nameof(FieldPlayerController.IsExistInteractiveIconByPosition))]
public static class FieldPlayerController_IsExistInteractiveIconByPosition
{
    public static void Prefix(FieldPlayerController __instance, ref IInteractiveEntity refInteractiveEntity, ref Il2CppSystem.Func<IInteractiveEntity, Boolean> conditionFunc)
    {
        Boolean willShowHiddenIcons = ModComponent.Instance.FieldControl.WillShowHiddenIcons();
        if (!willShowHiddenIcons)
            return;

        conditionFunc = new Filter(__instance, conditionFunc).Callback;
    }
    
    private sealed class Filter
    {
        private readonly FieldPlayerController _fieldPlayerController;
        private readonly Il2CppSystem.Func<IInteractiveEntity, Boolean> _func;
        public Il2CppSystem.Func<IInteractiveEntity, Boolean> Callback { get; }

        public Filter(FieldPlayerController fieldPlayerController, Il2CppSystem.Func<IInteractiveEntity, Boolean> func)
        {
            _fieldPlayerController = fieldPlayerController;
            _func = func;
            Callback = (System.Func<IInteractiveEntity, Boolean>)Func;
        }

        private Boolean Func(IInteractiveEntity entity)
        {
            if (_func != null && !_func.Invoke(entity))
                return false;
            
            FieldEntity interactive = entity.IntaractiveFieldEntity;
            Vector2 iPos = interactive.gameObject.transform.position;
            
            FieldPlayer player = _fieldPlayerController.fieldPlayer;
            Vector2 pPos = player.gameObject.transform.position;
            
            Vector2 pDir = player.directionVector;
            
            // 0 - teleport
            // 1 - chest
            // 2 - merchant
            for (Int32 i = 0; i < 5; i++)
            {
                if (Vector2.SqrMagnitude(pPos - iPos) < 0.001f)
                    return true;

                pPos += pDir;
            }

            return false;
        }
    }
}