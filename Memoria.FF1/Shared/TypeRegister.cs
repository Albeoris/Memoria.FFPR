using System;
using System.Reflection;
using BepInEx.Logging;
using Il2CppSystem.Collections.Generic;
using UnhollowerRuntimeLib;

namespace Memoria.FFPR
{
    public sealed class TypeRegister
    {
        private readonly ManualLogSource _log;

        public TypeRegister(ManualLogSource logSource)
        {
            _log = logSource ?? throw new ArgumentNullException(nameof(logSource));
        }

        public void RegisterRequiredTypes()
        {
            try
            {
                // Not supported :(
                
                // _log.LogInfo("Registering required types...");
                //
                // ClassInjector.RegisterTypeInIl2Cpp<Dictionary<String, String>>();
                //
                // _log.LogInfo($"1 additional types required successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register required types.", ex);
            }
        }

        public void RegisterAssemblyTypes()
        {
            try
            {
                _log.LogInfo("Registering assembly types...");

                Assembly assembly = Assembly.GetExecutingAssembly();
                Int32 count = RegisterTypes(assembly);
                
                _log.LogInfo($"{count} assembly types registered successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register assembly types.", ex);
            }
        }

        private Int32 RegisterTypes(Assembly assembly)
        {
            Int32 count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (!IsImportableType(type))
                    continue;

                try
                {
                    ClassInjector.RegisterTypeInIl2Cpp(type);
                    count++;
                }
                catch (Exception ex)
                {
                    _log.LogError($"Failed to register type {type.FullName}. Error: {ex}");
                    throw;
                }
            }

            return count;
        }

        private static Boolean IsImportableType(Type type)
        {
            if (!type.IsClass)
                return false;

            return type.Namespace?.EndsWith(".IL2CPP") ?? false;
        }
    }
}