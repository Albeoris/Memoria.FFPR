using System;
using System.Collections.Generic;
using Memoria.FFPR.IL2CPP;
using Newtonsoft.Json.Linq;

namespace Memoria.FF2.Internal.Core;

public static class ConversationManager
{
    private const String UsedWordsTag = "Memoria.FFPR.SecretWords.Used";
    private const String UsedItemsTag = "Memoria.FFPR.KeyItems.Used";

    private static Dictionary<String, HashSet<Int32>> _usedWords = new();
    private static String _entity;

    public static void ChangeLastInteractionEntity(FieldEntityId fieldEntity)
    {
        _entity = (fieldEntity ?? throw new ArgumentNullException(nameof(fieldEntity))).AsString();
    }

    public static void RememberUsedWord(Int32 uniqueId)
    {
        HashSet<Int32> usedWords = GetUsedWords();
        usedWords.Add(uniqueId);
    }

    public static Boolean WasUsedWord(Int32 uniqueId)
    {
        return HasUsedWords(out HashSet<Int32> words)
               && words.Contains(uniqueId);
    }

    private static Boolean HasUsedWords(out HashSet<Int32> words)
    {
        String currentEntity = EnsureEntityWasSet();

        return _usedWords.TryGetValue(currentEntity, out words);
    }

    private static HashSet<Int32> GetUsedWords()
    {
        String currentEntity = EnsureEntityWasSet();

        if (!_usedWords.TryGetValue(currentEntity, out var words))
        {
            words = new HashSet<Int32>();
            _usedWords.Add(currentEntity, words);
        }

        return words;
    }

    private static String EnsureEntityWasSet()
    {
        if (_entity is null)
            throw new InvalidOperationException("Interaction entity was not set.");

        return _entity;
    }

    public static void SaveData(JObject root)
    {
        root.Add(UsedWordsTag, JObject.FromObject(_usedWords));
    }

    public static void LoadData(JObject root)
    {
        JToken words = root[UsedWordsTag];
        _usedWords = words is null
            ? new Dictionary<String, HashSet<Int32>>()
            : words.ToObject<Dictionary<String, HashSet<Int32>>>();
    }

    public static void ResetData()
    {
        _usedWords = new Dictionary<String, HashSet<Int32>>();
    }
}