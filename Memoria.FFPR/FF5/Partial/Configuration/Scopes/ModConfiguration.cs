﻿using Memoria.FFPR.IL2CPP;

namespace Memoria.FFPR.Configuration.Scopes;

public sealed partial class ModConfiguration
{
    private partial void InitializeGameSpecificOptions(ConfigFileProvider provider)
    {
        if (Assets.ImportTextures)
        {
            Assets.ImportTextures = false;
            ModComponent.Log.LogWarning("Texture import is not supported: https://github.com/Albeoris/Memoria.FFPR/issues/37");
        }
    }
}