using System;
using System.Collections.Generic;
using Last.Data.Master;
using Last.Data.User;
using Last.Interpreter.Instructions.SystemCall;
using Last.UI.KeyInput;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.IL2CPP;
using Serial.FF6.UI.KeyInput;
using UnityEngine;

namespace Memoria.FFPR.FF6.Internal;

public sealed class RageAbilityColorization
{
    public static RageAbilityColorization Instance { get; } = new();
    
    private readonly List<OwnedAbility> _changedAbilities = new();
    private readonly HashSet<Int32> _changedAbilityIds = new();

    public void BeforeAbilityListShown(AbilityContentListController controller, Command command, OwnedCharacterData character)
    {
        if (!ModComponent.Instance.Config.BattleGau.DisplayUnacquiredRageAbilities)
            return;
        
        _changedAbilities.Clear();
        _changedAbilityIds.Clear();
        
        if (character.characterStatusId != (Int32)Current.CharacterStatusId.Gau)
            return;

        if (command.Id != CommandIds.Rage)
            return;

        foreach (OwnedAbility item in character.OwnedAbilityList)
        {
            if (item.SkillLevel == 0)
            {
                item.SkillLevel = 100;
                _changedAbilities.Add(item);
                _changedAbilityIds.Add(item.Ability.Id);
            }
        }
    }

    public void AfterAbilityListShown(AbilityContentListController controller, Command command, OwnedCharacterData character)
    {
        if (!ModComponent.Instance.Config.BattleGau.DisplayUnacquiredRageAbilities)
            return;
        
        if (_changedAbilities.Count == 0)
            return;
        
        foreach (OwnedAbility item in _changedAbilities)
            item.SkillLevel = 0;
    }

    public void AfterAbilityListUpdateView(AbilityContentListController controller)
    {
        if (!ModComponent.Instance.Config.BattleGau.DisplayUnacquiredRageAbilities)
            return;
        
        if (_changedAbilityIds.Count == 0)
            return;
        
        List<BattleAbilityInfomationContentController> contentList = controller.contentList.ToManaged();
        List<OwnedAbility> abilities = controller.AbilityList.ToManaged();
        for (var index = 0; index < contentList.Count; index++)
        {
            BattleAbilityInfomationContentController content = contentList[index];
            OwnedAbility ability = abilities[index];
            if (_changedAbilityIds.Contains(ability.Ability.Id))
            {
                Color32 color = ModComponent.Instance.Config.BattleGau.UnacquiredRageAbilitiesColor;
                content.SetTextColor(color);
            }
        }
    }
}