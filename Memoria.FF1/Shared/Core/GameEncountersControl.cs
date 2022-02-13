using System;
using Last.Map;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.IL2CPP;
using UnityEngine.SceneManagement;

namespace Memoria.FFPR.Core;

public sealed class GameEncountersControl
{
    public GameEncountersControl()
    {
    }

    private Boolean _isDisabled;
    private Boolean _isToggled;

    public Boolean DisableEncounters { get; private set; }

    public void Update()
    {
        try
        {
            if (_isDisabled)
                return;

            ProcessEncounters();
        }
        catch (Exception ex)
        {
            _isDisabled = true;
            ModComponent.Log.LogError($"[{nameof(GameEncountersControl)}].{nameof(Update)}(): {ex}");
        }
    }

    private void ProcessEncounters()
    {
        var config = ModComponent.Instance.Config.Encounters;

        var toggleKey = config.ToggleKey.Value;
        var toggleAction = config.ToggleAction.Value;

        var holdKey = config.HoldKey.Value;
        var holdAction = config.HoldAction.Value;

        Boolean isToggled = InputManager.IsToggled(toggleKey) || InputManager.GetKeyUp(toggleAction);
        Boolean isHold = InputManager.IsHold(holdKey) || InputManager.GetKey(holdAction);

        if (isToggled)
        {
            _isToggled = !_isToggled;
            if (_isToggled)
            {
                DisableEncounters = true;
                UpdateIndicator();
                return;
            }

            DisableEncounters = false;
        }

        if (!_isToggled)
            DisableEncounters = isHold;

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