using HarmonyLib;
using Last.Map;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(HiddenPassageController), nameof(HiddenPassageController.ObserveMoveFinishedFootPoint))]
public static class HiddenPassageController_ObserveMoveFinishedFootPoint
{
    public static void Postfix(HiddenPassageController __instance)
    {
        ModComponent.Instance.FieldControl.ShowHiddenPassages(__instance);
    }
}