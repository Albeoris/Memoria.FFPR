// using System;
// using Memoria.FF1.IL2CPP;
// using HarmonyLib;
// using Last.Map.Animation;
// using UnhollowerBaseLib;
//
// namespace Memoria.FF1.IL2CPP
// {
//     [HarmonyPatch(typeof(CharaDamageAnimation), "AnimSpeed", MethodType.Getter)]
//     internal class CharaDamageAnimation_AnimSpeed : Il2CppObjectBase
//     {
//         public CharaDamageAnimation_AnimSpeed(IntPtr ptr) : base(ptr)
//         {
//         }
//
//         public static void Postfix(ref Single __result)
//         {
//             ModComponent.Log.LogInfo($"AnimSpeed: {__result}");
//             __result = 10.0f;
//         }
//     }
// }