using System;
using Last.Map;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using UnityEngine.SceneManagement;

namespace Memoria.FFPR.Core;

public sealed class GameEncountersControl : SafeComponent
{
    private readonly HotkeyControl _disableEncountersKey = new();
    
    public GameEncountersControl()
    {
    }

    public Boolean DisableEncounters => _disableEncountersKey.XorHeldAndToggled;

    protected override void Update()
    {
        ProcessEncounters();
    }

    private void ProcessEncounters()
    {
        var config = ModComponent.Instance.Config.Encounters;
        if (_disableEncountersKey.Update(config.Key.Value))
            UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        if (DisableEncounters)
        {
            ModComponent.Instance.Drawer.Add("e");
        }
        else
        {
            ModComponent.Instance.Drawer.Remove("e");
        }
    }
}