using HarmonyLib;
using Il2CppSystem;
using Last.Management;
using Last.UI;
using Last.UI.KeyInput;
using Action = System.Action;
using Boolean = System.Boolean;
using String = System.String;

namespace Memoria.FFPR.IL2CPP.HarmonyHooks;

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