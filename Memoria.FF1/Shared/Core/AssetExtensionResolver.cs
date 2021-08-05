using System;
using System.IO;
using UnityEngine;

namespace Memoria.FFPR.Core
{
    public sealed class AssetExtensionResolver
    {
        private readonly String _catalogJson;

        public AssetExtensionResolver()
        {
            _catalogJson = File.ReadAllText(Application.streamingAssetsPath + @"/aa/catalog.json");
        }

        public String GetFileExtension(String assetAddress)
        {
            Int32 index = _catalogJson.IndexOf(assetAddress);
            if (index < 0)
                return String.Empty;

            Int32 lastIndex = index + assetAddress.Length;
            if (_catalogJson.Length <= lastIndex)
                return String.Empty;

            if (_catalogJson[lastIndex] != '.')
                return String.Empty;

            Int32 quoteIndex = _catalogJson.IndexOf('"', lastIndex + 1);
            if (quoteIndex < 0)
                return String.Empty;

            return _catalogJson.Substring(lastIndex, quoteIndex - lastIndex);
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