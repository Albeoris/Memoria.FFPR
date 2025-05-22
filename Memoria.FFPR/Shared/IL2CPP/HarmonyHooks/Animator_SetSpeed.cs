using System;
using HarmonyLib;
using Last.Entity.Field;
using UnityEngine;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(Animator), nameof(Animator.speed), MethodType.Setter)]
public static class Animator_SetSpeed
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

    static Exception Finalizer(Exception __exception)
    {
        return SuppressError ? null : __exception;
    }
}