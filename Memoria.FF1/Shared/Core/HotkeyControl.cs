using System;
using Memoria.FFPR.Configuration;
using UnityEngine;

namespace Memoria.FFPR.Core;

public sealed class HotkeyControl
{
    private HotkeyToggleState _toggle = HotkeyToggleState.None;
    private HotkeyHoldState _hold = HotkeyHoldState.None;
    private Single _holdOnTimeSeconds;

    public HotkeyControl()
    {
    }

    public Boolean ToggleChanged { get; private set; }
    public Boolean HoldChanged { get; private set; }
    public Boolean Changed => ToggleChanged || HoldChanged;

    public Boolean IsToggled => Toggle switch
    {
        HotkeyToggleState.None => false,
        HotkeyToggleState.ToggleOn => true,
        HotkeyToggleState.Toggled => true,
        HotkeyToggleState.ToggleOff => false,
        _ => throw new NotSupportedException(Toggle.ToString())
    };

    public Boolean IsHeld => Hold switch
    {
        HotkeyHoldState.None => false,
        HotkeyHoldState.HoldOn => true,
        HotkeyHoldState.Held => true,
        HotkeyHoldState.HoldOff => false,
        _ => throw new NotSupportedException(Hold.ToString())
    };

    public Boolean IsHeldOrToggled => IsHeld || IsToggled;

    public Boolean XorHeldAndToggled => IsHeld ^ IsToggled;

    public HotkeyToggleState Toggle
    {
        get => _toggle;
        private set
        {
            if (_toggle != value)
            {
                _toggle = value;
                ToggleChanged = true;
            }
            else
            {
                ToggleChanged = false;
            }
        }
    }

    public HotkeyHoldState Hold
    {
        get => _hold;
        private set
        {
            if (_hold != value)
            {
                _hold = value;
                HoldChanged = true;
            }
            else
            {
                HoldChanged = false;
            }
        }
    }

    public Single HeldSeconds => Hold == HotkeyHoldState.None
        ? 0.0f
        : Time.unscaledTime - _holdOnTimeSeconds;


    public Boolean Update(HotkeyGroup hotkeyGroup)
    {
        Boolean? isHeld = null;
        Boolean? isPressed = null;
        foreach (var group in hotkeyGroup.GroupedByHeld)
        {
            Boolean canToggle = true;
            foreach (Hotkey hotkey in group)
            {
                if (hotkey.MustHeld)
                {
                    if (InputManager.IsHold(hotkey))
                        isHeld = true;
                    else
                    {
                        isHeld ??= false;

                        if (HeldSeconds > 0.5f)
                            canToggle = false;
                    }
                }
                else
                {
                    if (canToggle && InputManager.IsToggled(hotkey))
                        isPressed = true;
                    else
                        isPressed ??= false;
                }
            }
        }

        UpdateStates(isHeld, isPressed);
        return Changed;
    }
    
    public Boolean Update(Hotkey hotkey)
    {
        Boolean? isHeld = null;
        Boolean? isPressed = null;
        if (hotkey.MustHeld)
        {
            isHeld = InputManager.IsHold(hotkey);
        }
        else
        {
            isPressed = InputManager.IsToggled(hotkey);
        }

        UpdateStates(isHeld, isPressed);
        return Changed;
    }

    private void UpdateStates(Boolean? isHeld, Boolean? isPressed)
    {
        Hold = UpdateHoldState(isHeld);
        Toggle = UpdateToggleState(isPressed);

        if (Hold == HotkeyHoldState.None)
            _holdOnTimeSeconds = 0.0f;
        else if (Hold == HotkeyHoldState.HoldOn)
            _holdOnTimeSeconds = Time.unscaledTime;
    }

    private HotkeyHoldState UpdateHoldState(Boolean? isHeld)
    {
        return isHeld switch
        {
            true => Hold switch
            {
                HotkeyHoldState.None => HotkeyHoldState.HoldOn,
                HotkeyHoldState.HoldOn => HotkeyHoldState.Held,
                HotkeyHoldState.Held => HotkeyHoldState.Held,
                HotkeyHoldState.HoldOff => HotkeyHoldState.HoldOn,
                _ => throw new NotSupportedException(Hold.ToString())
            },
            false => Hold switch
            {
                HotkeyHoldState.None => HotkeyHoldState.None,
                HotkeyHoldState.HoldOn => HotkeyHoldState.HoldOff,
                HotkeyHoldState.Held => HotkeyHoldState.HoldOff,
                HotkeyHoldState.HoldOff => HotkeyHoldState.None,
                _ => throw new NotSupportedException(Hold.ToString())
            },
            _ => Hold switch
            {
                HotkeyHoldState.None => HotkeyHoldState.None,
                HotkeyHoldState.HoldOn => HotkeyHoldState.HoldOff,
                HotkeyHoldState.Held => HotkeyHoldState.HoldOff,
                HotkeyHoldState.HoldOff => HotkeyHoldState.None,
                _ => throw new NotSupportedException(Hold.ToString())
            }
        };
    }
    
    private HotkeyToggleState UpdateToggleState(Boolean? isPressed)
    {
        return isPressed switch
        {
            true => Toggle switch
            {
                HotkeyToggleState.None => HotkeyToggleState.ToggleOn,
                HotkeyToggleState.ToggleOn => HotkeyToggleState.ToggleOff,
                HotkeyToggleState.Toggled => HotkeyToggleState.ToggleOff,
                HotkeyToggleState.ToggleOff => HotkeyToggleState.ToggleOn,
                _ => throw new NotSupportedException(Toggle.ToString())
            },
            false => Toggle switch
            {
                HotkeyToggleState.None => HotkeyToggleState.None,
                HotkeyToggleState.ToggleOn => HotkeyToggleState.Toggled,
                HotkeyToggleState.Toggled => HotkeyToggleState.Toggled,
                HotkeyToggleState.ToggleOff => HotkeyToggleState.None,
                _ => throw new NotSupportedException(Toggle.ToString())
            },
            _ => Toggle switch
            {
                HotkeyToggleState.None => HotkeyToggleState.None,
                HotkeyToggleState.ToggleOn => HotkeyToggleState.ToggleOff,
                HotkeyToggleState.Toggled => HotkeyToggleState.ToggleOff,
                HotkeyToggleState.ToggleOff => HotkeyToggleState.None,
                _ => throw new NotSupportedException(Toggle.ToString())
            },
        };
    }
}