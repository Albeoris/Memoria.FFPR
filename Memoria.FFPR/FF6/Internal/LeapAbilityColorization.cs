using System;
using System.Collections.Generic;
using System.Linq;
using Last.Data.Master;
using Last.Data.User;
using Last.Interpreter.Instructions;
using Last.Interpreter.Instructions.SystemCall;
using Last.UI.KeyInput;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.IL2CPP;
using UnityEngine.UI;

namespace Memoria.FFPR.FF6.Internal;

public sealed class LeapAbilityColorization
{
    public static LeapAbilityColorization Instance { get; } = new();

    private Boolean _hasUnlearnedAbilities;
    private Text _leapCommandText;
    private String _leapCommandTextValue;

    public void AfterBattleCommandSelectingInit(BattleMenuController controller)
    {
        if (!ModComponent.Instance.Config.BattleGau.ColorizeLeapCommand)
            return;
        
        if (controller is null)
            throw new ArgumentNullException(nameof(controller));
        
        _leapCommandText = FindLeapCommandText(controller);
        if (_leapCommandText is null)
        {
            _leapCommandTextValue = null;
            _hasUnlearnedAbilities = false;
        }
        else
        {
            _leapCommandTextValue = _leapCommandText.text;
            _hasUnlearnedAbilities = CheckHasUnlearnedAbilities();
        }
    }
    
    public void AfterBattleCommandSelectingUpdate(BattleMenuController controller)
    {
        BattleGauConfiguration config = ModComponent.Instance.Config.BattleGau;
        if (!config.ColorizeLeapCommand)
            return;
        
        Text text = _leapCommandText;
        if (text != null && text.isActiveAndEnabled && text.text == _leapCommandTextValue)
        {
            text.color = _hasUnlearnedAbilities
                ? config.LeapCommandSensibleColor
                : config.LeapCommandSenselessColor;
        }
    }

    private Text FindLeapCommandText(BattleMenuController controller)
    {
        BattleCommandSelectController selectController = controller.infomationController?.commandSelectController;
        if (selectController is null)
            return null;
        
        foreach (BattleCommandSelectContentController context in selectController.contentList)
        {
            Command targetCommand = context.TargetCommand;
            if (targetCommand == null)
                continue;
            
            if (targetCommand.Id == CommandIds.Leap)
                return context.GetComponentInChildren<Text>();
        }

        return null;
    }

    private static Boolean CheckHasUnlearnedAbilities()
    {
        IReadOnlyList<Int32> learnable = GetLearnableRageAbilities();
        if (learnable.Count == 0)
            return false;

        OwnedCharacterData gau = FindCharacter(Current.CharacterStatusId.Gau);
        if (gau is null)
            return false;

        HashSet<Int32> ownedAbilities = GetOwnedAbilities(gau);
        return learnable.Any(abilityId => !ownedAbilities.Contains(abilityId));
    }

    private static IReadOnlyList<Int32> GetLearnableRageAbilities()
    {
        Il2CppSystem.Collections.Generic.List<Int32> learnableMonsters = Current.GetLearningRampageEnemyList();
        Int32 learnableMonstersCount = learnableMonsters.Count;
        if (learnableMonstersCount == 0)
            return Array.Empty<Int32>();

        Il2CppSystem.Collections.Generic.Dictionary<Int32, Int32> abilityByMonster = Current.rampageAbilityList;
        HashSet<Int32> set = new HashSet<Int32>();
        List<Int32> abilities = new List<Int32>(capacity: learnableMonstersCount);
        foreach (Int32 monsterId in learnableMonsters)
        {
            if (!set.Add(monsterId))
                continue;

            abilities.Add(abilityByMonster[monsterId]);
        }

        return abilities;
    }

    private static OwnedCharacterData FindCharacter(Current.CharacterStatusId statusId)
    {
        return External.Character.GetOwnedCharacterByStatusId((Int32)statusId);
    }

    private static HashSet<Int32> GetOwnedAbilities(OwnedCharacterData character)
    {
        Il2CppSystem.Collections.Generic.List<OwnedAbility> abilities = character.OwnedAbilityList;
        HashSet<Int32> abilityIds = new();
        
        foreach (OwnedAbility ability in character.OwnedAbilityList)
        {
            if (ability.SkillLevel >= 100)
                abilityIds.Add(ability.Ability.Id);
        }

        return abilityIds;
    }
}