using System;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Input;
using Il2CppSystem.Input.KeyConfig;
using UnityEngine;

namespace Memoria.FF1.Core
{
    public static class InputManager
    {
        public static Boolean GetKey(KeyCode keyCode) => Check(keyCode, Input.GetKey);
        public static Boolean GetKeyDown(KeyCode keyCode) => Check(keyCode, Input.GetKeyDown);
        public static Boolean GetKeyUp(KeyCode keyCode) => Check(keyCode, Input.GetKeyUp);

        public static Boolean GetKey(String action) => Check(action, Input.GetKey);
        public static Boolean GetKeyDown(String action) => Check(action, Input.GetKeyDown);
        public static Boolean GetKeyUp(String action) => Check(action, Input.GetKeyUp);

        private static Boolean Check(KeyCode keyCode, Func<KeyCode, Boolean> checker)
        {
            return keyCode != KeyCode.None && checker(keyCode);
        }

        private static Boolean Check(String action, Func<KeyCode, Boolean> checker)
        {
            if (action == "None")
                return false;

            List<KeyValue> values = InputListener.Instance.KeyConfig.GetKeyValues(action);

            foreach (var value in values)
            {
                if (checker(value.KeyCode))
                    return true;
            }

            return false;
        }
    }
}