using System;
using System.Collections.Generic;
using Il2CppSystem.Input;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using Memoria.FFPR.IL2CPP.HarmonyHooks;
using UnityEngine;

namespace Memoria.FFPR.Core;

public sealed class GameSpeedControl : SafeComponent
{
    public GameSpeedControl()
    {
    }

    private readonly HotkeyControl _speedUpKey = new();
    private Single _nativeFactor;
    private Single _knownFactor;

    protected override void Update()
    {
        ProcessSpeedUp();
    }

    private void ProcessSpeedUp()
    {
        Single currentFactor = Time.timeScale;
        if (currentFactor == 0.0f) // Do not unpause the game 
            return;

        var config = ModComponent.Instance.Config.Speed;
        _speedUpKey.Update(config.Key.Value);

        if (currentFactor != _knownFactor)
            _nativeFactor = currentFactor;

        if (_speedUpKey.IsHeld)
        {
            _knownFactor = _nativeFactor * config.HoldFactor.Value;
            UpdateIndicator(flag: true);
        }
        else if (_speedUpKey.IsToggled)
        {
            _knownFactor = _nativeFactor * config.ToggleFactor.Value;
            UpdateIndicator(flag: true);
        }
        else
        {
            _knownFactor = _nativeFactor;
            UpdateIndicator(flag: false);
        }

        Time.timeScale = _knownFactor;
    }

    private static void UpdateIndicator(Boolean flag)
    {
        if (flag)
        {
            ModComponent.Instance.Drawer.Add("s");
        }
        else
        {
            ModComponent.Instance.Drawer.Remove("s");
        }
    }
}