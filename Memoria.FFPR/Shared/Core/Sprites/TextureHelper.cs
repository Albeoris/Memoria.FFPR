using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memoria.FFPR.IL2CPP;
using UnityEngine;

namespace Memoria.FFPR.Core;

public static class TextureHelper
{
    public static Texture2D GetFragment(Texture2D texture, Single x, Single y, Single width, Single height)
    {
        return GetFragment(texture,
            (Int32)Math.Round(x),
            (Int32)Math.Round(y),
            (Int32)Math.Round(width),
            (Int32)Math.Round(height));
    }

    public static Texture2D GetFragment(Texture2D texture, Int32 x, Int32 y, Int32 width, Int32 height)
    {
        if (texture == null)
            return null;

        Texture2D result = new Texture2D(width, height, texture.format, false);
        Color[] colors = texture.GetPixels(x, y, width, height);
        result.SetPixels(colors);
        result.Apply();
        return result;
    }

    public static Texture2D CopyAsReadable(Texture texture)
    {
        if (texture == null)
            return null;

        RenderTexture oldTarget = Camera.main.targetTexture;
        RenderTexture oldActive = RenderTexture.active;

        Texture2D result = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

        RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
        try
        {
            Camera.main.targetTexture = rt;
            //Camera.main.Render();
            Graphics.Blit(texture, rt);

            RenderTexture.active = rt;
            result.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        }
        finally
        {
            RenderTexture.active = oldActive;
            Camera.main.targetTexture = oldTarget;
            RenderTexture.ReleaseTemporary(rt);
        }

        return result;
    }

    public static void WriteTextureToFile(Texture2D texture, String outputPath)
    {
        Byte[] data;
        String extension = Path.GetExtension(outputPath) ?? String.Empty;
        switch (extension)
        {
            case "": // FF6 - Texture2D %StreamingAssets%\Assets/GameAssets/Serial/Res/UI/Common/Library/Thumbnail/MT_FF6_147_C00/Thumbnail
            case ".png":
                data = ImageConversion.EncodeToPNG(texture);
                break;
            case ".jpg":
                data = ImageConversion.EncodeToJPG(texture);
                break;
            case ".tga":
                data = ImageConversion.EncodeToTGA(texture);
                break;
            default:
                throw new NotSupportedException($"Not supported type [{extension}] of texture [{texture.name}]. Path: [{outputPath}]");
        }

        File.WriteAllBytes(outputPath, data);
    }

    public static Texture2D ReadTextureFromFile(FilterMode filterMode, String fullPath)
    {
        Byte[] bytes = File.ReadAllBytes(fullPath);
        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.filterMode = filterMode;
        if (!ImageConversion.LoadImage(texture, bytes))
            throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");
        return texture;
    }

    public static IEnumerable<IGrouping<Texture2D, Sprite>> GroupByTexture(Sprite[] sprites)
    {
        return sprites.GroupBy(s => s.texture);
    }
}