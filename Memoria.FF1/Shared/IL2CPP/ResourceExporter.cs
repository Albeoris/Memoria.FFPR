using System;
using System.Collections.Generic;
using System.IO;
using Last.Management;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Core;
using UnityEngine;
using Exception = System.Exception;
using IntPtr = System.IntPtr;

namespace Memoria.FFPR.IL2CPP
{
    public sealed class ResourceExporter : MonoBehaviour
    {
        private ResourceManager _resourceManager;

        private String _exportDirectory;
        private Int32 _currentIndex;
        private Int32 _totalCount = 1;
        private Boolean _enumeratorIsNull = true;
        private Dictionary<String, Dictionary<String, String>>.Enumerator _enumerator;
        private KeyValuePair<String, Dictionary<String, String>> _currentGroup;
        private DateTime _loadingStartTime;
        private DateTime _loadingLogTime;
        private AssetExtensionResolver _extensionResolver;
        private Texture2D _blackTexture;
        private GUIStyle _guiStyle;

        public ResourceExporter(IntPtr ptr) : base(ptr)
        {
        }

        public void Awake()
        {
            try
            {
                _exportDirectory = ModComponent.Instance.Config.Assets.ExportDirectory;
                if (_exportDirectory == String.Empty)
                {
                    ModComponent.Log.LogInfo($"[Export] Export skipped. Export directory is not defined.");
                    Destroy(this);
                    return;
                }

                Time.timeScale = 0.0f;

                ModComponent.Log.LogInfo($"[Export] Game stopped. Export started. Directory: {_exportDirectory}");
                ModComponent.Log.LogInfo($"[Export] Waiting for ResourceManager initialization.");
                _extensionResolver = new AssetExtensionResolver();

                // OnGui
                _blackTexture = new Texture2D(1, 1);
                _blackTexture.SetPixel(0, 0, Color.black);
                _blackTexture.Apply();

                _guiStyle = new GUIStyle();
                _guiStyle.fontSize = 48;
                _guiStyle.normal.textColor = Color.white;
                _guiStyle.alignment = TextAnchor.MiddleCenter;
            }
            catch (Exception ex)
            {
                OnExportError(ex);
            }
        }

        public void Update()
        {
            try
            {
                // Waiting for ResourceManager initialization
                if (_resourceManager is null)
                {
                    _resourceManager = ResourceManager.Instance;
                    if (_resourceManager is null)
                        return;

                    _resourceManager.LoadAssetBundle("AssetsPath");
                    ModComponent.Log.LogInfo($"[Export] Waiting for AssetsPath loading.");
                }

                // Waiting for AssetsPath loading
                if (_enumeratorIsNull)
                {
                    if (!_resourceManager.CheckLoadAssetCompleted("AssetsPath"))
                        return;

                    Dictionary<String, Dictionary<String, String>> dic = GetAssetsPath();
                    _totalCount = dic.Count;
                    _enumerator = dic.GetEnumerator();
                    _enumeratorIsNull = false;

                    ModComponent.Log.LogInfo($"[Export] Exporting assets {_totalCount} listed in AssetsPath...");
                }

                // Must have to export not readable textures
                if (Camera.main is null)
                    return;

                if (_currentGroup.Key != null)
                {
                    String assetGroup = _currentGroup.Key;
                    TimeSpan elapsedTime = DateTime.Now - _loadingLogTime;
                    if (!_resourceManager.CheckGroupLoadAssetCompleted(assetGroup))
                    {
                        if (elapsedTime.TotalSeconds > 5)
                        {
                            elapsedTime = DateTime.Now - _loadingStartTime;
                            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] Loading assets from [{assetGroup}]. Elapsed: {elapsedTime.TotalSeconds} sec.");
                            _loadingLogTime = DateTime.Now;
                        }

                        return;
                    }

                    elapsedTime = DateTime.Now - _loadingStartTime;
                    Dictionary<String, String> assets = _currentGroup.Value;
                    ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] Loaded {assets.Count} assets from [{assetGroup}] in {elapsedTime.TotalSeconds} sec. Exporting...");

                    Il2CppSystem.Collections.Generic.Dictionary<String, Il2CppSystem.Object> loaded = _resourceManager.completeAssetDic;
                    foreach (var pair in assets)
                    {
                        String assetName = pair.Key;
                        String assetPath = pair.Value;

                        Il2CppSystem.Object asset = loaded[assetPath];
                        if (asset is null)
                        {
                            ModComponent.Log.LogError($"[Export ({_currentIndex} / {_totalCount})] \tCannot find asset [{assetName}]: {assetPath}");
                            continue;
                        }

                        String extension = _extensionResolver.GetFileExtension(assetPath);
                        String type = _extensionResolver.GetAssetType(asset);

                        String exportPath = assetPath + extension;

                        ExportAsset(asset, type, assetName, exportPath);
                    }

                    _resourceManager.DestroyGroupAsset(assetGroup);
                    _currentGroup = default;
                }

                if (_enumerator.MoveNext())
                {
                    _loadingStartTime = DateTime.Now;
                    _loadingLogTime = _loadingStartTime;
                    _currentGroup = _enumerator.Current;

                    try
                    {
                        _currentIndex++;
                        _resourceManager.RequestGroupLoadAssetBundle(_currentGroup.Key);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to load assets from [{_currentGroup.Key}].", ex);
                    }
                }
                else
                {
                    ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] Assets exported successfully.");
                    if (ModComponent.Instance.Config.Assets.ExportAutoDisable)
                        ModComponent.Instance.Config.Assets.DisableExport();
                    Destroy(this);
                }
            }
            catch (Exception ex)
            {
                OnExportError(ex);
            }
        }

        public void OnGUI()
        {
            GUI.skin.box.normal.background = _blackTexture;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);

            Single progress = (100.0f * _currentIndex) / _totalCount;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), $"Exporting ({progress:F2}%): {_currentIndex} / {_totalCount}", _guiStyle);
        }

        public void OnDisable()
        {
            try
            {
                ModComponent.Log.LogInfo($"[Export] Export stopped.");
                if (_exportDirectory != String.Empty)
                    Application.Quit();
            }
            catch (Exception ex)
            {
                OnExportError(ex);
            }
        }

        private void ExportAsset(Il2CppSystem.Object asset, String type, String assetName, String assetPath)
        {
            String fullPath = Path.Combine(_exportDirectory, assetPath);

            Boolean overwrite = ModComponent.Instance.Config.Assets.ExportOverwrite;
            if (!overwrite && File.Exists(fullPath))
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting an existing file of asset [{assetName}]: {shortPath}");
                return;
            }

            switch (type)
            {
                case "UnityEngine.AnimationClip":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.AnimatorOverrideController":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.GameObject":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.Material":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.RenderTexture":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.RuntimeAnimatorController":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.Shader":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.Sprite":
                    ExportDummy(type, assetName);
                    break;
                case "UnityEngine.TextAsset":
                    ExportText(asset.Cast<TextAsset>(), fullPath);
                    break;
                case "System.Byte[]":
                    ExportBinary(asset.Cast<TextAsset>(), fullPath);
                    break;
                case "UnityEngine.Texture2D":
                    ExportTexture2D(assetName, asset.Cast<Texture2D>(), fullPath);
                    break;
                case "UnityEngine.U2D.SpriteAtlas":
                    ExportDummy(type, assetName);
                    break;
            }
        }

        private void ExportText(TextAsset asset, String fullPath)
        {
            String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
            if (!ModComponent.Instance.Config.Assets.ExportText)
            {
                ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting [{asset.name}] TextAsset: disabled in config file.");
                return;
            }

            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tExport [{asset.name}] TextAsset {shortPath}");
            PrepareDirectory(fullPath);
            File.WriteAllText(fullPath, asset.text);
        }

        private void ExportTexture2D(String assetName, Texture2D asset, String fullPath)
        {
            String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
            if (!ModComponent.Instance.Config.Assets.ExportTextures)
            {
                ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting [{asset.name}] Texture2D: disabled in config file.");
                return;
            }

            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tExport [{asset.name}] Texture2D {shortPath}");

            PrepareDirectory(fullPath);

            if (asset.isReadable)
            {
                TextureHelper.WriteTextureToFile(asset, fullPath);
            }
            else
            {
                Texture2D readable = TextureHelper.CopyAsReadable(asset);
                TextureHelper.WriteTextureToFile(readable, fullPath);
                Destroy(readable);
            }
        }

        private void ExportBinary(TextAsset asset, String fullPath)
        {
            String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
            if (!ModComponent.Instance.Config.Assets.ExportBinary)
            {
                ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting [{asset.name}] BinaryAsset: disabled in config file.");
                return;
            }

            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tExport [{asset.name}] BinaryAsset {shortPath}");
            PrepareDirectory(fullPath);
            File.WriteAllBytes(fullPath, asset.bytes);
        }

        void ExportDummy(String type, String assetName)
        {
            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip {assetName}. Not supported type: {type}");
        }

        private static void PrepareDirectory(String fullPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        }

        private void OnExportError(Exception exception)
        {
            ModComponent.Log.LogError($"[Export] Failed to export assets: {exception}");
            Destroy(this);
        }

        private Dictionary<String, Dictionary<String, String>> GetAssetsPath()
        {
            TextAsset assetPathAsset = _resourceManager.completeAssetDic["AssetsPath"].Cast<TextAsset>();
            if (assetPathAsset is null)
                throw new Exception("[Export] Cannot find text resource AssetsPath.");

            var il2cppDic = AssetPathUtilty.Parse(assetPathAsset.text);
            return il2cppDic.ToManaged();
        }
    }
}