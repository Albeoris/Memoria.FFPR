using System;
using System.IO;
using Memoria.FFPR.IL2CPP;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Memoria.FFPR.Core
{
    public sealed class AssetExtensionResolver
    {
        public AssetExtensionResolver()
        {
        }

        public String GetFileExtension(String assetAddress)
        {
            String extension = ContentCatalogData_CreateLocator.GetFileExtension(assetAddress);
            return extension;
        }

        public String GetAssetType(Il2CppSystem.Object asset)
        {
            String type = asset.GetIl2CppType().FullName;
            
            if (type == "UnityEngine.TextAsset")
            {
                TextAsset textAsset = asset.Cast<TextAsset>();
                if (textAsset.text == String.Empty && textAsset.bytes.Length != 0)
                    type = "System.Byte[]";
            }

            return type;
        }
    }
}