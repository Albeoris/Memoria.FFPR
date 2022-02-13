using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Il2CppSystem;
using Last.Data;
using Last.Entity.Field;
using Last.Management;
using Last.Map;
using Last.UI;
using Last.UI.KeyInput;
using Memoria.FFPR.IL2CPP;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using Action = System.Action;
using Boolean = System.Boolean;
using Exception = System.Exception;
using Int32 = System.Int32;
using IntPtr = System.IntPtr;
using Object = System.Object;
using String = System.String;

// ReSharper disable InconsistentNaming

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

[HarmonyPatch(typeof(FieldPlayerController), nameof(FieldPlayerController.IsExistInteractiveIconByPosition))]
public static class FieldPlayerController_IsExistInteractiveIconByPosition
{
    public static void Prefix(FieldPlayerController __instance, ref IInteractiveEntity refInteractiveEntity, ref Func<IInteractiveEntity, Boolean> conditionFunc)
    {
        Boolean willShowHiddenIcons = ModComponent.Instance.FieldControl.WillShowHiddenIcons();
        if (!willShowHiddenIcons)
            return;

        conditionFunc = new Filter(__instance, conditionFunc).Callback;
    }
    
    private sealed class Filter
    {
        private readonly FieldPlayerController _fieldPlayerController;
        private readonly Func<IInteractiveEntity, Boolean> _func;
        public Func<IInteractiveEntity, Boolean> Callback { get; }

        public Filter(FieldPlayerController fieldPlayerController, Func<IInteractiveEntity, Boolean> func)
        {
            _fieldPlayerController = fieldPlayerController;
            _func = func;
            Callback = (System.Func<IInteractiveEntity, Boolean>)Func;
        }

        private Boolean Func(IInteractiveEntity entity)
        {
            if (_func != null && !_func.Invoke(entity))
                return false;
            
            FieldEntity interactive = entity.IntaractiveFieldEntity;
            Vector2 iPos = interactive.gameObject.transform.position;
            
            FieldPlayer player = _fieldPlayerController.fieldPlayer;
            Vector2 pPos = player.gameObject.transform.position;
            
            Vector2 pDir = player.directionVector;
            
            // 0 - teleport
            // 1 - chest
            // 2 - merchant
            for (Int32 i = 0; i < 5; i++)
            {
                if (Vector2.SqrMagnitude(pPos - iPos) < 0.001f)
                    return true;

                pPos += pDir;
            }

            return false;
        }
    }
}

[HarmonyPatch(typeof(HiddenPassageController), nameof(HiddenPassageController.ObserveMoveFinishedFootPoint))]
public static class HiddenPassageController_ObserveMoveFinishedFootPoint
{
    public static void Postfix(HiddenPassageController __instance)
    {
        ModComponent.Instance.FieldControl.ShowHiddenPassages(__instance);
    }
}

[HarmonyPatch(typeof(FieldMap), nameof(FieldMap.UpdatePlayerStatePlay))]
public static class FieldMap_UpdatePlayerStatePlay
{
    private static readonly Action EmptyAction = () => { };
    private static readonly Il2CppSystem.Action YesCpp = (Action)OnYes;
    private static readonly Il2CppSystem.Action NoCpp = (Action)OnNo;

    private static Action _yesAction = EmptyAction;
    private static Action _noAction = EmptyAction;
    private static CommonPopup _popupWindow;

    public static Boolean CanAsk => _popupWindow is null;

    public static Boolean Prefix(FieldMap __instance)
    {
        CommonPopup popup = _popupWindow;
        if (popup is null)
            return true;

        popup.UpdateCommand();
        return false;
    }

    public static void Postfix()
    {
        ModComponent.Instance.FieldControl.ShowHiddenIcons();
    }

    public static void Ask(String title, String question, Action yesAction)
    {
        _yesAction = yesAction;
        _noAction = EmptyAction;
        
        PopupManager popupManager = SingletonMonoBehaviour<PopupManager>.Instance;
        CommonPopup popup = popupManager.GetOrCreate<CommonPopup>();

        popup.Title = title;
        popup.Message = question;
        popup.SetPositiveCommand(MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.SPEECH_IN_OPTION_YES),
            YesCpp);
        popup.SetNegativeCommand(MessageManager.Instance.GetMessageByMessageConclusion(UiMessageConstants.SPEECH_IN_OPTION_NO),
            NoCpp);
        popup.Open();
        popup.ResetCursor();
        popup.UpdateSelect();
        _popupWindow = popup;
    }

    private static void OnYes()
    {
        try
        {
            _yesAction();
        }
        finally
        {
            CloseWindow();
        }
    }

    private static void OnNo()
    {
        try
        {
            _noAction();
        }
        finally
        {
            CloseWindow();
        }
    }
    
    private static void CloseWindow()
    {
        CommonPopup window = _popupWindow;
        _popupWindow = null;

        window?.Close();
        _yesAction = EmptyAction;
        _noAction = EmptyAction;
    }
}


[HarmonyPatch(typeof(ContentCatalogData), nameof(ContentCatalogData.CreateLocator))]
public sealed class ContentCatalogData_CreateLocator : Il2CppSystem.Object
{
    private static readonly Dictionary<String, Dictionary<String,String>> KnownCatalogs = new();
        
    public ContentCatalogData_CreateLocator(IntPtr ptr) : base(ptr)
    {
    }

    public static String GetFileExtension(String assetAddress)
    {
        // ReSharper disable once StringLiteralTypo
        return GetFileExtension("AddressablesMainContentCatalog", assetAddress);
    }

    public static String GetFileExtension(String catalogName, String assetAddress)
    {
        return
            KnownCatalogs.TryGetValue(catalogName, out var dictionary)
            && dictionary.TryGetValue(assetAddress, out var extension)
                ? extension
                : String.Empty;
    }

    public static void Prefix(ContentCatalogData __instance)
    {
        String locatorId = __instance.location.PrimaryKey;
        if (KnownCatalogs.ContainsKey(locatorId))
            return;

        Dictionary<String, String> extensions = new();
        KnownCatalogs.Add(locatorId, extensions);

        try
        {
            foreach (String path in __instance.InternalIds)
            {
                String extension = Path.GetExtension(path);
                String pathWithoutExtension = path.Substring(0, path.Length - extension.Length);
                extensions[pathWithoutExtension] = extension;
            }
                
            ModComponent.Log.LogInfo($"[ContentCatalogData_CreateLocator] Resource file extensions have been successfully parsed.");
        }
        catch (Exception ex)
        {
            ModComponent.Log.LogError($"[ContentCatalogData_CreateLocator] Failed to parse resource file extensions from catalog {__instance.location.InternalId}. Error: {ex}");
        }
    }
}