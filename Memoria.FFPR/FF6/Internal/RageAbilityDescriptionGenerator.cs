using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Last.Data.Master;
using Last.Defaine.Master;
using Last.Management;
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

        RageAbilityFormatter rageAbilityFormatter = new(masterManager);
        RageSpeciesFormatter rageSpeciesFormatter = new(MessageManager.Instance);
        RageResistanceFormatter rageResistanceFormatter = new(masterManager);

        Ability[] allAbilities = GetAllRageAbilities(masterManager);
        // Dictionary<Int32, AbilityRandomGroup[]> randomGroups = GetAllRandomGroups(masterManager);

        StringBuilder sb = new StringBuilder();

        foreach (Ability ability in allAbilities)
        {
            Int32 monsterId = ability.DataA;
            if (monsterId < 1)
                continue;

            Monster monster = masterManager.GetData<Monster>(monsterId);
            if (monster is null)
                continue;

            sb.Clear();
            
            String abilities = rageAbilityFormatter.FormatAbilities(monster);
            String race = rageSpeciesFormatter.FormatSpecies(monster);
            String initial = rageResistanceFormatter.FormatInitialCondition(monster);
            String resistance = rageResistanceFormatter.FormatResistance(monster);

            if (sb.Length > 0) sb.AppendLine();
            sb.Append(abilities);

            sb.Append(' ', 8);
            sb.Append(race);
            sb.Append(' ');

            if (!String.IsNullOrEmpty(initial))
            {
                sb.Append('[');
                sb.Append(initial);
                sb.Append(']');
            }

            if (sb.Length > 0) sb.AppendLine();
            sb.Append(resistance);
           
            _abilityDescriptions.Add(ability.Id, sb.ToString());
        }
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