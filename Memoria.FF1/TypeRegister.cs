using System;
using System.Reflection;
using BepInEx.Logging;
using Il2CppSystem.Collections.Generic;
using UnhollowerRuntimeLib;

namespace Memoria.FF1
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
                // ClassInjector.RegisterTypeInIl2Cpp<Dictionary<String, IntPtr>>();
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

                MethodInfo registrator = typeof(ClassInjector).GetMethod("RegisterTypeInIl2Cpp", new Type[0]);
                if (registrator == null)
                    throw new Exception("Cannot find method RegisterTypeInIl2Cpp.");
                
                Assembly assembly = Assembly.GetExecutingAssembly();
                Int32 count = RegisterTypes(assembly, registrator);
                
                _log.LogInfo($"{count} assembly types registered successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to register assembly types.", ex);
            }
        }

        private Int32 RegisterTypes(Assembly assembly, MethodInfo registrator)
        {
            Int32 count = 0;
            var parameters = new object[0];

            foreach (Type type in assembly.GetTypes())
            {
                if (!IsImportableType(type))
                    continue;

                MethodInfo genericMethod = registrator.MakeGenericMethod(type);
                genericMethod.Invoke(null, parameters);
                count++;
            }

            return count;
        }

        private static Boolean IsImportableType(Type type)
        {
            return type.FullName?.StartsWith("Memoria.FF1.IL2CPP") ?? false;
        }
    }
}