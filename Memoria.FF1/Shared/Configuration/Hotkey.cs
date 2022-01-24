using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.FFPR.Configuration;

public sealed class Hotkey
{
    public KeyCode Key { get; set; } = KeyCode.None;
    public Boolean Alt { get; set; }
    public Boolean Shift { get; set; }
    public Boolean Control { get; set; }
    public IReadOnlyList<KeyCode> Modifiers { get; set; } = Array.Empty<KeyCode>();
}