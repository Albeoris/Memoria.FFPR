using System;
using System.Collections.Generic;

namespace Memoria.FFPR.BeepInEx;

public static class ExtensionMethods
{
    public static void Deconstruct<TKey, TValue>(this Il2CppSystem.Collections.Generic.KeyValuePair<TKey, TValue> il2cpp, out TKey key, out TValue value)
    {
        key = il2cpp.Key;
        value = il2cpp.Value;
    }
    
    public static Dictionary<TKey1, Dictionary<TKey2, TValue>> ToManaged<TKey1, TKey2, TValue>(
        this Il2CppSystem.Collections.Generic.Dictionary<TKey1, Il2CppSystem.Collections.Generic.Dictionary<TKey2, TValue>> il2cpp)
    {
        return il2cpp.ToManaged(k => k, v => v.ToManaged());
    }

    public static Dictionary<TKey, TValue> ToManaged<TKey, TValue>(this Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> il2cpp)
    {
        return il2cpp.ToManaged(k => k, v => v);
    }

    public static Dictionary<TTargetKey, TTargetValue> ToManaged<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(
        this Il2CppSystem.Collections.Generic.Dictionary<TSourceKey, TSourceValue> il2cpp,
        Func<TSourceKey, TTargetKey> keySelector,
        Func<TSourceValue, TTargetValue> valueSelector)
    {
        if (il2cpp is null) throw new ArgumentNullException(nameof(il2cpp));
        if (keySelector is null) throw new ArgumentNullException(nameof(keySelector));
        if (valueSelector is null) throw new ArgumentNullException(nameof(valueSelector));

        if (il2cpp.comparer.Pointer != Il2CppSystem.Collections.Generic.EqualityComparer<TSourceKey>.Default.Pointer)
            throw new ArgumentException($"The IL2CPP Dictionary uses a non-standard Comparer ([{il2cpp.comparer}]) that cannot be converted to a Managed type.", nameof(il2cpp));

        var result = new Dictionary<TTargetKey, TTargetValue>(il2cpp.Count);

        foreach ((TSourceKey k, TSourceValue v) in il2cpp)
            result.Add(keySelector(k), valueSelector(v));

        return result;
    }
}