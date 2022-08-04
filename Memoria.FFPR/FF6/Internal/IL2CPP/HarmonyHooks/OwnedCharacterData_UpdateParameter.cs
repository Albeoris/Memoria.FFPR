using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HarmonyLib;
using Last.Data.User;
using Last.Interpreter.Instructions.SystemCall;
using Last.Systems;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FF6.Internal.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(OwnedCharacterData), nameof(OwnedCharacterData.characterStatusId), MethodType.Getter)]
public sealed class OwnedCharacterData_UpdateParameter : Il2CppSystem.Object
{
    public OwnedCharacterData_UpdateParameter(IntPtr ptr) : base(ptr)
    {
    }
    
    public static void Postfix(OwnedCharacterData __instance, Int32 __result)
    {
        try
        {
            AbilityOrder sortOrder = ModComponent.Instance.Config.BattleGau.RageAbilitiesSortOrder;
            if (sortOrder == AbilityOrder.Default)
                return;
            
            if (Interlocked.Read(ref OwnedCharacterClient_AddFromJson.IsInvoked) != 1)
                return;
            
            if (__result != (Int32)Current.CharacterStatusId.Gau)
                return;
            
            foreach (var pair in __instance.AbilityDictionary)
            {
                if (pair.Key.Id != CommandIds.Rage)
                    continue;
                
                Il2CppSystem.Collections.Generic.List<OwnedAbility> list = pair.Value;
                IEnumerable<OwnedAbility> ordered = list.ToManaged();
                if (sortOrder == AbilityOrder.Alphabetical)
                    ordered = ordered.OrderBy(i => ContentUtitlity.GetAbilityName(i.Ability));
                else
                    throw new NotSupportedException(sortOrder.ToString());
                
                list.Clear();
                foreach (var item in ordered)
                {
                    item.ability.SortId = list.Count;
                    list.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogException(ex);
        }
    }
}