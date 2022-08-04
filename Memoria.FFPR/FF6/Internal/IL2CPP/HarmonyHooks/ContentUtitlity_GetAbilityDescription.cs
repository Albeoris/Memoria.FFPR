using System;
using HarmonyLib;
using Last.Data.Master;
using Last.Systems;
using Memoria.FFPR.FF6.Internal;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(ContentUtitlity), nameof(ContentUtitlity.GetAbilityDescription), typeof(Ability))]
public sealed class ContentUtitlity_GetAbilityDescription : Il2CppSystem.Object
{
    public ContentUtitlity_GetAbilityDescription(IntPtr ptr) : base(ptr)
    {
    }

    public static void Postfix(Ability ability, ref String __result)
    {
        // Don't change existing description
        if (!String.IsNullOrEmpty(__result))
            return;

        if (RageAbilityDescriptionGenerator.Instance.TryGetAbilityDescription(ability.Id, out String description))
            __result = description;
    }
}