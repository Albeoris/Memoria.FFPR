using System;
using System.Collections;
using System.Collections.Generic;

namespace Memoria.FFPR.Core;

public sealed class OrderedSet<T> : IEnumerable<T>
{
    private readonly IEqualityComparer<T> _comparer;
    private readonly HashSet<T> _set;
    private readonly List<T> _list;

    public OrderedSet()
        : this(EqualityComparer<T>.Default)
    {
    }

    public OrderedSet(IEqualityComparer<T> comparer)
    {
        _comparer = comparer;
        _set = new HashSet<T>(comparer);
        _list = new List<T>();
    }

    public Boolean TryAdd(T item)
    {
        if (_set.Add(item))
        {
            _list.Add(item);
            return true;
        }

        return false;
    }

    public Boolean TryRemove(T item)
    {
        if (!_set.Remove(item))
            return false;

        for (var index = 0; index < _list.Count; index++)
        {
            T existing = _list[index];
            if (_comparer.Equals(existing, item))
            {
                _list.RemoveAt(index);
                return true;
            }
        }

        return false;
    }

    public Boolean Contains(T item)
    {
        return _set.Contains(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}