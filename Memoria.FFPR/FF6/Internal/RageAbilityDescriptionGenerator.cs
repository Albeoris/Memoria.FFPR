using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Last.Data.Master;
using Last.Defaine.Master;
using Last.Systems;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.IL2CPP;

namespace Memoria.FFPR.FF6.Internal;

public sealed class RageAbilityDescriptionGenerator
{
    public static RageAbilityDescriptionGenerator Instance { get; } = new();

    private readonly Regex _textTagRegex = new("<[^>]+?>", RegexOptions.Compiled);
    private readonly Dictionary<Int32, String> _abilityDescriptions = new();

    public RageAbilityDescriptionGenerator()
    {
        if (!ModComponent.Instance.Config.BattleGau.GenerateRageAbilityDescription)
            return;
        
        Initialize();
    }

    public Boolean TryGetAbilityDescription(Int32 abilityId, out String description)
    {
        if (!ModComponent.Instance.Config.BattleGau.GenerateRageAbilityDescription)
        {
            description = null;
            return false;
        }
        
        return _abilityDescriptions.TryGetValue(abilityId, out description);
    }

    private void Initialize()
    {
        MasterManager masterManager = MasterManager.Instance;

        Ability[] allAbilities = GetAllRageAbilities(masterManager);
        Dictionary<Int32, AbilityRandomGroup[]> randomGroups = GetAllRandomGroups(masterManager);

        StringBuilder sb = new StringBuilder();

        foreach (Ability ability in allAbilities)
        {
            Int32 monsterId = ability.DataA;
            if (monsterId < 1)
                continue;

            Monster monster = masterManager.GetData<Monster>(monsterId);
            if (monster is null)
                continue;

            if (!randomGroups.TryGetValue(monster.RageAbilityRandomGroupId, out AbilityRandomGroup[] randomGroup))
                continue;

            sb.Clear();

            foreach (AbilityRandomGroup randomAbility in randomGroup)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                String abilityName = ContentUtitlity.GetAbilityName(randomAbility.AbilityId);
                abilityName = _textTagRegex.Replace(abilityName, String.Empty);
                sb.Append($"{abilityName} ({randomAbility.InvocationRate}{(randomAbility.ConditionGroupId < 1 ? '%' : '?')})");
            }

            _abilityDescriptions.Add(ability.Id, sb.ToString());
        }
    }

    private static Dictionary<Int32, AbilityRandomGroup[]> GetAllRandomGroups(MasterManager masterManager)
    {
        return masterManager
            .GetList<AbilityRandomGroup>()
            .ToManaged()
            .Select(g => g.Value)
            .GroupBy(g => g.GroupId)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    private static Ability[] GetAllRageAbilities(MasterManager masterManager)
    {
        return masterManager
            .GetList<Ability>()
            .ToManaged()
            .Values
            .Where(a => (AbilityType)a.TypeId == AbilityType.Rampage)
            .ToArray();
    }
}