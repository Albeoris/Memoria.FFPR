using System;
using System.Collections.Generic;
using System.Linq;
using Last.Entity.Field;
using Last.Management;
using Last.Map;
using Memoria.FFPR.IL2CPP;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Memoria.FFPR.Core;

public sealed class GameEntitiesControl : SafeComponent
{
    private readonly HotkeyControl _showInteractionIconsKey = new();
    
    public GameEntitiesControl()
    {
    }

    protected override void Update()
    {
        UpdateInput();
    }

    private void UpdateInput()
    {
        if (_showInteractionIconsKey.Update(ModComponent.Instance.Config.Field.HighlightingKey.Value))
            UpdateIndicator(WillShowHiddenIcons());
    }
    
    public Boolean WillShowHiddenIcons()
    {
        return _showInteractionIconsKey.XorHeldAndToggled;
    }

    public void ShowHiddenIcons()
    {
        if (!WillShowHiddenIcons())
            return;
        
        var types = ModComponent.Instance.Config.Field.HighlightingIcons.Value;
        if (types.Count == 0)
            return;

        EntityManager entityManager = EntityManager.Instance;
        if (entityManager is null)
        {
            ModComponent.Log.LogWarning("[GameEntitiesControl] EntityManager is null.");
            return;
        }
        
        var entities = EntityManager.Instance.GetRegisterEntityList();

        foreach (var entity in entities)
        {
            FieldInteractiveIcon icon = entity.TryCast<FieldInteractiveIcon>();
            if (icon is null)
                continue;

            if (icon.IsActive)
                continue;
            
            if (!types.Contains(icon.IconType))
                continue;
            
            icon.Show();
            icon.UpdateEntity();
        }
    }

    private Boolean _isHiddenPassagesShown;
    
    public void ShowHiddenPassages(HiddenPassageController controller)
    {
        Boolean show = ModComponent.Instance.Config.Field.HighlightHiddenPassages.Value
                       && _showInteractionIconsKey.XorHeldAndToggled;

        if (show)
        {
            foreach (var pair in controller.currentHiddenPassageGroupList)
            {
                HiddenPassageGroup value = pair.Value;
                value.Show();
            }

            _isHiddenPassagesShown = true;
        }
        else if (_isHiddenPassagesShown)
        {
            foreach (var pair in controller.currentHiddenPassageGroupList)
            {
                HiddenPassageGroup value = pair.Value;
                value.Hide();
            }

            _isHiddenPassagesShown = false;
        }
    }
    
    private static void UpdateIndicator(Boolean flag)
    {
        if (flag)
        {
            ModComponent.Instance.Drawer.Add("i");
        }
        else
        {
            ModComponent.Instance.Drawer.Remove("i");
        }
    }
}