using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Last.Management;
using Memoria.FFPR.BeepInEx;
using Memoria.FFPR.Configuration;
using Memoria.FFPR.Configuration.Scopes;
using Memoria.FFPR.Core;
using Microsoft.DotNet.Tools.Common;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.U2D;
using Exception = System.Exception;
using IntPtr = System.IntPtr;

namespace Memoria.FFPR.IL2CPP;

public sealed class ResourceExporter : MonoBehaviour
{
    private ResourceManager _resourceManager;

    private String _exportDirectory;
    private Int32 _currentIndex;
    private Int32 _totalCount = 1;
    private Int32 _skippedCount;
    private Int32 _notSupportedCount;
    private Boolean _enumeratorIsNull = true;
    //private Dictionary<String, Dictionary<String, String>>.Enumerator _enumerator;
    private IEnumerator _enumerator;
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
            AssetsConfiguration config = ModComponent.Instance.Config.Assets;
            _exportDirectory = config.GetExportDirectoryIfEnabled();
            if (_exportDirectory == String.Empty)
            {
                ModComponent.Log.LogInfo($"[Export] Export skipped. Export directory is not defined.");
                Destroy(this);
                return;
            }
            
            SpriteAtlasCache.IsDisabled = true;
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

                List<(String assetGroup, String assetName, String exportPath, SpriteAtlas atlas)> atlasList = new();
                List<(String assetName, String exportPath, Sprite sprite)> spriteList = new();
                Boolean exportTextures = ModComponent.Instance.Config.Assets.ExportTextures;
                
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
                    
                    if (exportTextures)
                    {
                        if (type == "UnityEngine.U2D.SpriteAtlas")
                        {
                            atlasList.Add((assetGroup, assetName, exportPath, asset.Cast<SpriteAtlas>()));
                        }
                        else if (type == "UnityEngine.Sprite")
                        {
                            spriteList.Add((assetName, exportPath, asset.Cast<Sprite>()));
                            continue;
                        }
                    }

                    ExportAsset(asset, type, assetName, exportPath);
                }

                for (var index = spriteList.Count - 1; index >= 0; index--)
                {
                    (String assetName, String exportPath, Sprite sprite) = spriteList[index];
                    
                    Sprite atlasSprite = null;
                    String existingAtlasPath = null;
                    String existingAtlasGroup = null;
                    String existingAtlasName = null;
                    foreach (var atlas in atlasList)
                    {
                        Sprite found = atlas.atlas.GetSprite($"{sprite.name}(Clone)")
                                       ?? atlas.atlas.GetSprite(sprite.name);

                        if (found != null)
                        {
                            if (atlasSprite is null)
                            {
                                atlasSprite = found;
                                existingAtlasPath = atlas.exportPath;
                                existingAtlasGroup = atlas.assetGroup;
                                existingAtlasName = atlas.assetName;
                            }
                            else
                            {
                                atlasSprite = null;
                                existingAtlasPath = null;
                                existingAtlasGroup = null;
                                existingAtlasName = null;
                                ModComponent.Log.LogWarning($"[Export ({_currentIndex} / {_totalCount})] \tFound few atlases with same sprites. Sprite will be exported as standalone [{assetName}]: {exportPath}");
                                break;
                            }
                        }
                    }

                    if (atlasSprite is null)
                    {
                        ExportAsset(sprite, "UnityEngine.Sprite", assetName, exportPath);
                    }
                    else
                    {
                        ExportAtlasReference(existingAtlasPath, existingAtlasGroup, existingAtlasName, atlasSprite.name, exportPath);
                    }
                }

                _resourceManager.DestroyGroupAsset(assetGroup);
                _currentGroup = default;
            }

            // Debug
            // Debug:
            if (_enumerator.MoveNext())
            {
                _loadingStartTime = DateTime.Now;
                _loadingLogTime = _loadingStartTime;
                _currentGroup = (KeyValuePair<String, Dictionary<String, String>>)_enumerator.Current;

                try
                {
                    _currentIndex++;
                    
                    // Debug
                    // if (!String.Equals(_currentGroup.Key, "MO_FF2_B024_C00", StringComparison.InvariantCultureIgnoreCase))
                    //     goto Debug;
                    
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
                if (_skippedCount > 0)
                {
                    ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] {_skippedCount} assets are skipped as they have already been exported before.");
                    if (!ModComponent.Instance.Config.Assets.ExportLogAlreadyExportedAssets)
                        ModComponent.Log.LogInfo($"For detailed information, set the option Assets.ExportLogAlreadyExportedAssets = true");
                }

                if (_notSupportedCount > 0)
                {
                    ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] {_notSupportedCount} assets skipped as they have unsupported types (shaders, materials, etc.)");
                    if (!ModComponent.Instance.Config.Assets.ExportLogNotSupportedAssets)
                        ModComponent.Log.LogInfo($"For detailed information, set the option Assets.ExportLogNotSupportedAssets = true");
                }

                if (ModComponent.Instance.Config.Assets.ExportAutoDisable)
                    ModComponent.Instance.Config.Assets.ExportEnabled = false;
                Destroy(this);
            }
        }
        catch (Exception ex)
        {
            OnExportError(ex);
        }
    }

    private void ExportAtlasReference(String atlasPath, String atlasAssetGroup, String atlasAssetName, String spriteName, String exportPath)
    {
        String atlasFullPath = Path.Combine(_exportDirectory, atlasPath);
        String spriteFullPath = Path.Combine(_exportDirectory, exportPath);
        String atlasRelativePath = PathUtility.GetRelativePath(spriteFullPath, atlasFullPath);

        String fullPath = spriteFullPath + ".ini";
        String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);

        Boolean overwrite = ModComponent.Instance.Config.Assets.ExportOverwrite;
        if (HandleExistingFile(spriteName, fullPath, overwrite))
            return;

        ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tExport [{spriteName}] Sprite {shortPath}");
        PrepareDirectory(fullPath);

        using (StreamWriter sw = File.CreateText(fullPath))
        {
            sw.WriteLine($"[Sprite]");
            sw.WriteLine($"AtlasPath = {atlasRelativePath}");
            sw.WriteLine($"SpriteName = {spriteName}");
            sw.WriteLine();
            sw.WriteLine($"[Asset]");
            sw.WriteLine($"AtlasGroup = {atlasAssetGroup}");
            sw.WriteLine($"AtlasName = {atlasAssetName}");
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
            {
                SpriteAtlasCache.IsDisabled = false;
                Application.Quit();
            }
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

        if (HandleExistingFile(assetName, fullPath, overwrite))
            return;

        switch (type)
        {
            case "UnityEngine.AnimationClip":
                ExportDummy(type, assetName, fullPath);
                break;
            case "UnityEngine.AnimatorOverrideController":
                ExportDummy(type, assetName, fullPath);
                break;
            case "UnityEngine.GameObject":
                ExportDummy(type, assetName, fullPath);
                break;
            case "UnityEngine.Material":
                ExportDummy(type, assetName, fullPath);
                break;
            case "UnityEngine.RenderTexture":
                ExportDummy(type, assetName, fullPath);
                break;
            case "UnityEngine.RuntimeAnimatorController":
                ExportDummy(type, assetName, fullPath);
                break;
            case "UnityEngine.Shader":
                ExportDummy(type, assetName, fullPath);
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
                ExportSpriteAtlas(assetName, asset.Cast<SpriteAtlas>(), fullPath, overwrite);
                break;
            case "UnityEngine.Sprite":
                ExportSprite(assetName, asset.Cast<Sprite>(), fullPath);
                break;
        }
    }

    private Boolean HandleExistingDirectory(String assetName, String fullPath, Boolean overwrite)
    {
        if (!Directory.Exists(fullPath))
            return false;
        
        if (overwrite)
        {
            if (!fullPath.Contains("FINAL FANTASY"))
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogWarning($"[Export ({_currentIndex} / {_totalCount})] \tDirectory {fullPath} cannot be deleted. Delete it manually if you think it's safe.");
                ModComponent.Log.LogWarning($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting an existing directory of asset [{assetName}]: {shortPath}");
                _skippedCount++;
                return true;
            }

            Directory.Delete(fullPath, recursive: true);
        }
        else
        {
            if (ModComponent.Instance.Config.Assets.ExportLogAlreadyExportedAssets)
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting an existing directory of asset [{assetName}]: {shortPath}");
            }
            _skippedCount++;

            return true;
        }

        return false;
    }
    
    private Boolean HandleExistingFile(String assetName, String fullPath, Boolean overwrite)
    {
        if (HandleExistingFile($"Export ({_currentIndex} / {_totalCount})", assetName, fullPath, overwrite))
        {
            _skippedCount++;
            return true;
        }

        return false;
    }

    public static Boolean HandleExistingFile(String logPrefix, String assetName, String fullPath, Boolean overwrite)
    {
        if (!File.Exists(fullPath))
            return false;
        
        if (overwrite)
        {
            if (!fullPath.Contains("FINAL FANTASY"))
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogWarning($"[{logPrefix}] \tFile {fullPath} cannot be deleted. Delete it manually if you think it's safe.");
                ModComponent.Log.LogWarning($"[{logPrefix}] \tSkip exporting an existing directory of asset [{assetName}]: {shortPath}");
                return true;
            }
        }
        else
        {
            if (ModComponent.Instance.Config.Assets.ExportLogAlreadyExportedAssets)
            {
                String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
                ModComponent.Log.LogInfo($"[{logPrefix}] \tSkip exporting an existing file of asset [{assetName}]: {shortPath}");
            }
            return true;
        }

        return false;
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

    private void ExportSpriteAtlas(String assetName, SpriteAtlas atlas, String atlasDirectoryPath, Boolean overwrite)
    {
        String shortPath = ApplicationPathConverter.ReturnPlaceholders(atlasDirectoryPath);
        if (!ModComponent.Instance.Config.Assets.ExportTextures)
        {
            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting [{assetName}] SpriteAtlas: disabled in config file.");
            return;
        }

        ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tExport [{assetName}] SpriteAtlas {shortPath}");
        
        if (HandleExistingDirectory(assetName, atlasDirectoryPath, overwrite))
            return;

        Directory.CreateDirectory(atlasDirectoryPath);
        String textureFileName = Path.ChangeExtension(Path.GetFileName(atlasDirectoryPath), ".png");
        String tpsheetFileName = Path.ChangeExtension(Path.GetFileName(atlasDirectoryPath), ".tpsheet");

        Int32 spriteCount = atlas.spriteCount;
        Il2CppReferenceArray<Sprite> spriteArray = new(spriteCount);
        atlas.GetSprites(spriteArray);

        Sprite[] sprites = spriteArray.ToManaged();

        Texture2D knownTexture = null;
        Texture2D texture = null;
        String sourcesPath = String.Empty;
        foreach (Sprite sprite in sprites)
        {
            if (knownTexture is null)
            {
                knownTexture = sprite.texture;
                texture = knownTexture.isReadable ? knownTexture : TextureHelper.CopyAsReadable(knownTexture);
                texture.name = textureFileName;

                String texturePath = Path.Combine(atlasDirectoryPath, texture.name);
                TextureHelper.WriteTextureToFile(texture, texturePath);

                sourcesPath = Path.Combine(atlasDirectoryPath, "Sources");
                Directory.CreateDirectory(sourcesPath);
            }
            else if (knownTexture != sprite.texture)
            {
                throw new NotSupportedException("Sprite atlases with multiple textures are not supported.");
            }

            Rect rect = sprite.GetTextureRect(); // don't use sprite.textureRect, can be empty!
            Texture2D fragment = TextureHelper.GetFragment(texture, rect.x, rect.y, rect.width, rect.height);
            TextureHelper.WriteTextureToFile(fragment, Path.Combine(sourcesPath, sprite.name + ".png"));
        }

        using (StreamWriter sw = File.CreateText(Path.Combine(atlasDirectoryPath, tpsheetFileName)))
        {
            sw.WriteLine($":format=40300");
            sw.WriteLine($":texture={texture.name}");
            sw.WriteLine($":size={texture.width}x{texture.height}");
            sw.WriteLine($":pivotpoints=enabled");
            sw.WriteLine($":borders=enabled");
            sw.WriteLine($":alphahandling=KeepTransparentPixels");
            sw.WriteLine();

            foreach (Sprite sprite in sprites)
            {
                RectInt rect = sprite.GetPixelTextureRect(); // don't use sprite.textureRect, can be empty!
                sw.Write(FormattableString.Invariant($"{sprite.name};{rect.x};{rect.y};{rect.width};{rect.height}; "));

                Vector2 pivot = sprite.GetMeshPivot();
                if (rect.width == 0 || rect.height == 0)
                    sw.Write(FormattableString.Invariant($"{0};{0}; "));
                else
                    sw.Write(FormattableString.Invariant($"{pivot.x};{pivot.y}; "));

                Vector4 border = sprite.border; // left, right, top, bottom
                sw.Write(FormattableString.Invariant($"{border.x};{border.z};{border.w};{border.y}"));

                Vector2Int[] vertices = sprite.GetPixelVertices();
                sw.Write($"; {vertices.Length}");
                foreach (Vector2 v in vertices)
                    sw.Write(FormattableString.Invariant($";{v.x};{v.y}"));

                UInt16[] triangles = sprite.triangles.ToManaged();
                if (triangles.Length % 3 != 0)
                    throw new NotSupportedException("triangles.Length % 3 != 0");
                
                sw.Write($"; {triangles.Length / 3}");
                foreach (UInt16 v in triangles)
                {
                    sw.Write(';');
                    sw.Write(v);
                }

                sw.WriteLine();
            }
        }

        if (!knownTexture.isReadable)
            Destroy(texture);

        // Debug
        // SpriteAtlasCache.Import(atlas, atlasDirectoryPath);
        // SpriteAtlasCache.IsDisabled = false;
        // spriteCount = atlas.spriteCount;
        // spriteArray = new(spriteCount);
        // atlas.GetSprites(spriteArray);
        // SpriteAtlasCache.IsDisabled = true;
    }
    
    private void ExportSprite(String assetName, Sprite sprite, String fullPath)
    {
        fullPath = Path.ChangeExtension(fullPath, ".png");

        String shortPath = ApplicationPathConverter.ReturnPlaceholders(fullPath);
        if (!ModComponent.Instance.Config.Assets.ExportTextures)
        {
            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip exporting [{sprite.name}] Sprite: disabled in config file.");
            return;
        }

        ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tExport [{sprite.name}] Sprite {shortPath}");
        PrepareDirectory(fullPath);
        
        Texture2D texture = sprite.texture;
        if (texture.isReadable)
        {
            Rect rect = sprite.GetTextureRect(); // don't use sprite.textureRect, can be empty!;
            Texture2D fragment = TextureHelper.GetFragment(texture, rect.x, rect.y, rect.width, rect.height);
            TextureHelper.WriteTextureToFile(fragment, fullPath);
        }
        else
        {
            Texture2D readable = TextureHelper.CopyAsReadable(texture);
            Rect rect = sprite.GetTextureRect(); // don't use sprite.textureRect, can be empty!;
            Texture2D fragment = TextureHelper.GetFragment(readable, rect.x, rect.y, rect.width, rect.height);
            TextureHelper.WriteTextureToFile(fragment, fullPath);
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

    void ExportDummy(String type, String assetName, String fullPath)
    {
        if (ModComponent.Instance.Config.Assets.ExportLogNotSupportedAssets)
            ModComponent.Log.LogInfo($"[Export ({_currentIndex} / {_totalCount})] \tSkip {assetName}. Not supported type: {type}");

        _notSupportedCount++;
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

        String exportPath = "Assets/GameAssets/AssetsPath.json";
        String fullPath = Path.Combine(_exportDirectory, exportPath);
        if (!HandleExistingFile("AssetsPath", fullPath, ModComponent.Instance.Config.Assets.ExportOverwrite))
            ExportText(assetPathAsset, fullPath); 

        var il2cppDic = AssetPathUtilty.Parse(assetPathAsset.text);
        return il2cppDic.ToManaged();
    }
}