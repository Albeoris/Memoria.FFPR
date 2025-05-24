using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Input;
using Il2CppSystem.Input.KeyConfig;
using Memoria.FFPR.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Memoria.FFPR.Core;

public static class InputManager
{
    public static Boolean GetKey(KeyCode keyCode) => Check(keyCode, Input.GetKey);
    public static Boolean GetKeyDown(KeyCode keyCode) => Check(keyCode, Input.GetKeyDown);
    public static Boolean GetKeyUp(KeyCode keyCode) => Check(keyCode, Input.GetKeyUp);

    public static Boolean GetKey(String action) => ActionPressed(action);
    public static Boolean GetKeyDown(String action) => ActionDown(action);
    public static Boolean GetKeyUp(String action) => ActionUp(action);

    public static Boolean IsToggled(Hotkey hotkey)
    {
        if (hotkey.Key == KeyCode.None && hotkey.Action == "None")
        {
            return false;
        }
        else if (hotkey.Key != KeyCode.None && !Input.GetKeyUp(hotkey.Key))
        {
            return false;
        }
        else if (hotkey.Action != "None" && !ActionUp(hotkey.Action))
        {
            return false;
        }

        return IsModifiersPressed(hotkey);
    }
    
    public static Boolean IsHold(Hotkey hotkey)
    {
        if (hotkey.Key == KeyCode.None && hotkey.Action == "None")
        {
            return false;
        }
        else if (hotkey.Key != KeyCode.None && !Input.GetKey(hotkey.Key))
        {
            return false;
        }
        else if (hotkey.Action != "None" && !ActionPressed(hotkey.Action))
        {
            return false;
        }

        return IsModifiersPressed(hotkey);
    }

    private static Boolean IsModifiersPressed(Hotkey hotkey)
    {
        if (hotkey.Control)
        {
            if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                return false;
        }

        if (hotkey.Alt)
        {
            if (!(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                return false;
        }

        if (hotkey.Shift)
        {
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                return false;
        }

        return hotkey.ModifierKeys.All(Input.GetKey) && hotkey.ModifierActions.All(ActionPressed);
    }

    private static Boolean ActionPressed(string action)
    {
        if (action == "None")
            return false;

        if (PlayerInput.all[0].actions.FindAction(action) is InputAction inputAction)
        {
            if (!inputAction.enabled)
            {
                inputAction.Enable();
            }
            return inputAction.IsPressed();
        }

        //List<KeyValue> values = InputListener.Instance.KeyConfig.GetKeyValues(action);

        //foreach (var value in values)
        //{
        //    if (Input.GetKey(value.KeyCode))
        //        return true;
        //}

        return false;
    }

    private static Boolean ActionDown(string action)
    {
        if (action == "None")
            return false;

        if (PlayerInput.all[0].actions.FindAction(action) is InputAction inputAction)
        {
            if (!inputAction.enabled)
            {
                inputAction.Enable();
            }
            return inputAction.WasPressedThisFrame();
        }

        //List<KeyValue> values = InputListener.Instance.KeyConfig.GetKeyValues(action);

        //foreach (var value in values)
        //{
        //    if (Input.GetKeyDown(value.KeyCode))
        //        return true;
        //}

        return false;
    }

    private static Boolean ActionUp(string action)
    {
        if (action == "None")
            return false;

        if (PlayerInput.all[0].actions.FindAction(action) is InputAction inputAction)
        {
            if (!inputAction.enabled)
            {
                inputAction.Enable();
            }
            return inputAction.WasReleasedThisFrame();
        }

        //List<KeyValue> values = InputListener.Instance.KeyConfig.GetKeyValues(action);

        //foreach (var value in values)
        //{
        //    if (Input.GetKeyUp(value.KeyCode))
        //        return true;
        //}

        return false;
    }
    private static Boolean Check(KeyCode keyCode, Func<KeyCode, Boolean> checker)
    {
        return keyCode != KeyCode.None && checker(keyCode);
    }
}