using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Last.Data.Master;
using Last.Systems;
using Last.UI;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.FF6.Internal;

public sealed class RageAbilityFormatter
{
    private readonly StringBuilder _sb = new();
    private readonly Dictionary<Int32, AbilityRandomGroup[]> _randomGroups;

    public RageAbilityFormatter(MasterManager master)
    {
        _randomGroups = GetAllRandomGroups(master);
    }

    public String FormatAbilities(Monster monster)
    {
        if (!_randomGroups.TryGetValue(monster.RageAbilityRandomGroupId, out AbilityRandomGroup[] randomGroup))
            return String.Empty;

        _sb.Clear();

        List<(String name, Boolean hasCondition, Int32 chance)> chances = new();

        foreach (AbilityRandomGroup randomAbility in randomGroup)
        {
            String abilityName = ContentUtitlity.GetAbilityName(randomAbility.AbilityId);
            abilityName = IconTextUtility.GetRepraceIconText(abilityName);
            chances.Add((abilityName, randomAbility.ConditionGroupId > 0, randomAbility.InvocationRate)); 
        }
        
        foreach (var group in chances.GroupBy(t=>(t.name, t.hasCondition)))
        {
            if (_sb.Length > 0)
                _sb.Append(", ");

            String name = group.Key.name;
            Boolean hasCondition = group.Key.hasCondition;
            Int32 chance = group.Sum(t => t.chance);
            _sb.Append($"{name} ({chance}{(hasCondition ? '?' : '%')})");
        }

        return _sb.ToString();
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
}