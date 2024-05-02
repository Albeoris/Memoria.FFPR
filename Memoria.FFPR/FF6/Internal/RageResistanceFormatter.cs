using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Last.Data.Master;
using Last.Data.Parameters;
using Last.Defaine;
using Last.Management;
using Last.Systems;
using Last.UI;
using Last.UI.Common.Library;
using Memoria.FFPR.BeepInEx;

namespace Memoria.FFPR.FF6.Internal;

public sealed class RageResistanceFormatter
{
    private readonly StringBuilder _sb = new();
    private readonly Dictionary<Int32, String> _elementNames = new();
    private readonly Dictionary<Int32, String> _conditionNames = new();
    private readonly MasterManager _master;
    private readonly MessageManager _messages;
    private readonly Dictionary<Int32, ConditionGroup[]> _allConditionGroups;

    public String TextWeakness { get; }
    public String TextResistance { get; }
    public String TextImmune { get; }
    public String TextAbsorbs { get; }

    public RageResistanceFormatter(MasterManager masterManager)
    {
        _master = masterManager;
        _messages = MessageManager.Instance;
        _allConditionGroups = GetAllConditionGroups(masterManager);
        TextWeakness = MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.MONSTER_BOOK_WEAK_POINT);
        TextResistance = MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.MONSTER_BOOK_RESISTANCE);
        TextAbsorbs = MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.MONSTER_BOOK_ABSORPTION);
        TextImmune = MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.MONSTER_BOOK_INVALID);
    }

    public String FormatInitialCondition(Monster monster)
    {
        if (!_allConditionGroups.TryGetValue(monster.InitialCondition, out ConditionGroup[] groups))
            return String.Empty;

        _sb.Clear();
        
        foreach (ConditionGroup group in groups)
        {
            String conditionName = ResolveConditionName(group.ConditionId);
            if (String.IsNullOrEmpty(conditionName))
                continue;

            if (_sb.Length > 0)
                _sb.Append(", ");
            _sb.Append(conditionName);
        }

        if (_sb.Length == 0)
            return String.Empty;

        return _sb.ToString();
    }

    public String FormatResistance(Monster monster)
    {
        // Dirty hack for FF6 where GroupMasterUtility returns IReadOnlyDictionary instead of Dictionary
        var attributeDic = GroupMasterUtility.GetResistanceAttributeDic(monster.ResistanceAttribute)
            .Cast<Il2CppSystem.Collections.Generic.Dictionary<Int32, Int32>>();
        var conditionDic = GroupMasterUtility.GetResistanceConditionDic(monster.ResistanceCondition)
            .Cast<Il2CppSystem.Collections.Generic.Dictionary<Int32, Int32>>();
        
        Dictionary<ResistanceAttributeType, List<Int32>> attributes = Group<ResistanceAttributeType>(attributeDic);
        Dictionary<ResistanceConditionType, List<Int32>> conditions = Group<ResistanceConditionType>(conditionDic);

        List<(String ResistanceType, List<String> Values)> translated = Translate(attributes, conditions);

        if (translated.Count == 0)
            return String.Empty;

        _sb.Clear();
        
        foreach ((String resistanceType, List<String> values) in translated)
        {
            if (_sb.Length > 0)
            {
                _sb.Length -= 2;
                _sb.Append(' ', 4);
            }

            _sb.Append(resistanceType);
            _sb.Append(": ");
            foreach (String value in values)
            {
                _sb.Append(value);
                _sb.Append(", ");
            }
        }

        _sb.Length -= 2;
        return _sb.ToString();
    }

    private String ResolveElementName(Int32 elementId)
    {
        if (!_elementNames.TryGetValue(elementId, out String name))
        {
            name = LibraryInfoContent.GetMessageConclusion($"MONSTER_BOOK_ATTRIBUTE_CATEGORY_{elementId}");
            name = IconTextUtility.GetRepraceIconText(name);
            _elementNames.Add(elementId, name);
        }

        return name;
    }

    private String ResolveConditionName(Int32 conditionId)
    {
        if (!_conditionNames.TryGetValue(conditionId, out String name))
        {
            Condition condition = _master.GetData<Condition>(conditionId);
            
            if (condition.MesIdName != "None")
            {
                name = _messages.GetMessage(condition.MesIdName);
                name = IconTextUtility.GetRepraceIconText(name);
            }
            else if (!UnnamedConditionNames.TryGetValue((LangugeUtility.GetLanguageType(), conditionId), out name))
            {
            }
            
            _conditionNames.Add(conditionId, name);
        }

        return name;
    }
    
    private List<(String ResistanceType, List<String> Values)> Translate(Dictionary<ResistanceAttributeType, List<Int32>> attributes, Dictionary<ResistanceConditionType, List<Int32>> conditions)
    {
        List<(String ResistanceType, List<String> Values)> result = new(capacity: 4);
        List<String> weak = new List<String>();
        List<String> absorb = new List<String>();
        List<String> immune = new List<String>();
        List<String> resist = new List<String>();

        foreach (var pair in attributes)
        {
            String sufix = String.Empty;
            List<String> target;
            switch (pair.Key)
            {
                case ResistanceAttributeType.Weak:
                    target = weak;
                    break;
                case ResistanceAttributeType.Half:
                    target = resist;
                    break;
                case ResistanceAttributeType.Invalid:
                    target = immune;
                    break;
                case ResistanceAttributeType.Absorption:
                    target = absorb;
                    break;
                case ResistanceAttributeType.DoubleWeak:
                    target = weak;
                    sufix = "*2";
                    break;
                case ResistanceAttributeType.DoubleHalf:
                    target = resist;
                    sufix = "/2";
                    break;
                default:
                    continue;
            }

            foreach (Int32 elementId in pair.Value)
            {
                String elementName = ResolveElementName(elementId);
                if (String.IsNullOrEmpty(elementName))
                    continue;
                        
                target.Add($"{elementName}{sufix}");
            }
        }
        
        foreach (var pair in conditions)
        {
            List<String> target;
            switch (pair.Key)
            {
                case ResistanceConditionType.Resist:
                    target = resist;
                    break;
                default:
                    continue;
            }
            
            foreach (Int32 conditionId in pair.Value)
            {
                String conditionName = ResolveConditionName(conditionId);
                if (String.IsNullOrEmpty(conditionName))
                    continue;
                
                target.Add(conditionName);
            }
        }
        
        if (weak.Count > 0) result.Add((TextWeakness, weak));
        if (absorb.Count > 0) result.Add((TextAbsorbs, absorb));
        if (immune.Count > 0) result.Add((TextImmune, immune));
        if (resist.Count > 0) result.Add((TextResistance, resist));
        return result;
    }

// #if FF6
//     private static Dictionary<T, List<Int32>> Group<T>(Il2CppSystem.Collections.Generic.IReadOnlyDictionary<Int32, Int32> attributes) where T : Enum
// #else
    private static Dictionary<T, List<Int32>> Group<T>(Il2CppSystem.Collections.Generic.Dictionary<Int32, Int32> attributes) where T : Enum
//#endif
    {
        Dictionary<T, List<Int32>> result = new();

        foreach (var pair in attributes)
        {
            var elementId = pair.Key;
            var type = (T)(object)pair.Value;

            if (!result.TryGetValue(type, out var list))
            {
                list = new List<Int32>();
                result.Add(type, list);
            }

            list.Add(elementId);
        }

        foreach (var list in result.Values)
            list.Sort();

        return result;
    }
    
    private static Dictionary<Int32, ConditionGroup[]> GetAllConditionGroups(MasterManager masterManager)
    {
        return masterManager
            .GetList<ConditionGroup>()
            .ToManaged()
            .Select(g => g.Value)
            .GroupBy(g => g.GroupId)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    private static readonly Dictionary<(Language language, Int32 conditionId), String> UnnamedConditionNames = new()
    {
        {(Language.En, 96), "Float"},
        {(Language.Ja, 96), "レビテト"},
        {(Language.Fr, 96), "Lévitation"},
        {(Language.It, 96), "Levitazione"},
        {(Language.De, 96), "Levitas"},
        {(Language.Es, 96), "Lévita"},
        {(Language.Ko, 96), "레비테트"},
        {(Language.Zht,96), "浮空"},
        {(Language.Zhc,96), "浮空"},
        {(Language.Ru, 96), "Полет"},
        {(Language.Th, 96), "ลอย"},
        {(Language.Pt, 96), "Flutuar"},
    };
}