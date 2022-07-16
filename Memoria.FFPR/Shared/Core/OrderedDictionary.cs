using System;
using System.Collections;
using System.Collections.Generic;

namespace Memoria.FFPR.Core;

public sealed class OrderedDictionary<TKey, TValue> : IEnumerable<(TKey key, TValue value)>
{
    private readonly Dictionary<TKey, (Int32 index, TValue value)> _set;
    private readonly List<(TKey key, TValue value)> _list;

    public OrderedDictionary()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public OrderedDictionary(IEqualityComparer<TKey> comparer)
    {
        _set = new Dictionary<TKey, (Int32 index, TValue value)>(comparer);
        _list = new List<(TKey key, TValue value)>();
    }

    public void AddOrUpdate(TKey key, TValue value)
    {
        if (TryAdd(key, value)) return;
        if (TryReplace(key, value, out _)) return;
        throw new InvalidOperationException("Collection is out of sync.");
    }

    public Boolean TryAdd(TKey key, TValue value)
    {
        if (_set.ContainsKey(key))
            return false;

        _set.Add(key, (_list.Count, value));
        _list.Add((key, value));
        return true;
    }
    
    public Boolean TryReplace(TKey key, TValue value, out TValue previousValue)
    {
        if (!_set.TryGetValue(key, out var pair))
        {
            previousValue = default;
            return false;
        }

        previousValue = pair.value;
        _set[key] = (pair.index, value);
        _list[pair.index] = (key, value);
        return true;
    }

    public Boolean TryRemove(TKey key, out TValue value)
    {
        if (!_set.TryGetValue(key, out var pair))
        {
            value = default;
            return false;
        }

        _set.Remove(key);
        _list.RemoveAt(pair.index);
        value = pair.value;
        return true;
    }

    public Boolean ContainsKey(TKey key)
    {
        return _set.ContainsKey(key);
    }

    public IEnumerator<(TKey key, TValue value)> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}