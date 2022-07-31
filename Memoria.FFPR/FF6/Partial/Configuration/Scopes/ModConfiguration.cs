﻿using System;
using Memoria.FFPR.IL2CPP;
using UnityEngine.Playables;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed partial class ModConfiguration
{
    public MagiciteConfiguration Magicite { get; private set; }
    public BattleBlitzConfiguration BattleBlitz { get; set; }

    private partial void InitializeGameSpecificOptions(ConfigFileProvider provider)
    {
        Magicite = MagiciteConfiguration.Create(provider);
        BattleBlitz = BattleBlitzConfiguration.Create(provider);

        if (Assets.ImportTextures)
        {
            Assets.ImportTextures = false;
            ModComponent.Log.LogWarning("Texture import is not supported: https://github.com/Albeoris/Memoria.FFPR/issues/23");
        }
    }
}