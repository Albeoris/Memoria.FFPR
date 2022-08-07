using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Last.Data.Master;
using Last.Management;

namespace Memoria.FFPR.FF6.Internal;

public sealed class RageSpeciesFormatter
{
    private readonly List<String> _species = new();
    private readonly StringBuilder _sb = new();
    private readonly Int32 _sbLength;

    public RageSpeciesFormatter(MessageManager message)
    {
        _species.Add(message.GetMessageByMessageConclusion("MONSTER_BOOK_CATEGORY_NONE"));
        _sb.Append(message.GetMessageByMessageConclusion("MONSTER_BOOK_SPECIES"));
        _sb.Append(": ");
        _sbLength = _sb.Length;
        
        for (int i = 1; i < 32; i++)
            _species.Add(message.GetMessageByMessageConclusion($"MONSTER_BOOK_SPECIES_CATEGORY_{i}"));
        while (_species.Count > 0 && String.IsNullOrEmpty(_species.Last()))
            _species.RemoveAt(_species.Count - 1);
    }

    public String FormatSpecies(Monster monster)
    {
        if (_species.Count == 0)
            return String.Empty;
        
        _sb.Length = _sbLength;

        Int32 mask = monster.Species;
        if (mask == 0)
        {
            _sb.Append(_species[0]);
            return _sb.ToString();
        }

        for (Int32 i = 0; i < _species.Count; i++)
        {
            Int32 value = (1 << i);
            if ((mask & value) == value)
            {
                _sb.Append(_species[i]);
                _sb.Append(", ");
            }
        }

        _sb.Length -= 2;
        return _sb.ToString();
    }
}