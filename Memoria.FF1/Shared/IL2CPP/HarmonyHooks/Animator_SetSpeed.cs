using System;
using HarmonyLib;
using Last.Entity.Field;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// ReSharper disable once InconsistentNaming
[HarmonyPatch(typeof(Animator), nameof(Animator.speed), MethodType.Setter)]
public sealed class Animator_SetSpeed : Il2CppSystem.Object
{
    /// <summary>
    /// Allows you to suppress an error when changing the animation speed of an icon during a fast loading game.
    /// </summary>
    public static Boolean SuppressError
    {
        get => _suppressErrorRequestNumber > 0;
        set
        {
            if (value)
                _suppressErrorRequestNumber++;
            else
                _suppressErrorRequestNumber--;
        }
    }

    private static Int32 _suppressErrorRequestNumber;

    public Animator_SetSpeed(IntPtr ptr) : base(ptr)
    {
    }

    static Exception Finalizer(Exception __exception)
    {
        return SuppressError ? null : __exception;
    }
}