using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Memoria.FFPR.IL2CPP;
using UnityEngine;

namespace Memoria.FFPR.Core;

public sealed class SpriteSheetInfo
{
    public Int32 Format { get; private set; }
    public String DataPath { get; private set; }
    public String TexturePath { get; private set; }
    public (Int32 Width, Int32 Height) AtlasSize { get; private set; }
    public Boolean IsPivotsEnabled { get; private set; }
    public Boolean IsBordersEnabled { get; private set; }
    public IReadOnlyList<SpriteInfo> Sprites => _sprites;

    private readonly List<SpriteInfo> _sprites = new();

    public static SpriteSheetInfo ReadFromFile(String dataPath)
    {
        return new SpriteSheetInfo(dataPath);
    }

    private SpriteSheetInfo(String dataPath)
    {
        DataPath = dataPath;
        ReadData(dataPath);
    }

    private void ReadData(String sheetPath)
    {
        using (StreamReader sr = File.OpenText(sheetPath))
        {
            while (!sr.EndOfStream)
            {
                String line = sr.ReadLine();

                if (TrySkipComments(line))
                    continue;

                if (TryParseHeader(line))
                    continue;

                String[] parts = line.Split(';');

                Int32 idx = 0;
                String spriteName = ParseName(parts, ref idx, nameof(spriteName));
                Rect textureRect = ParseRect(parts, ref idx, nameof(textureRect));
                Vector2 pivot = ParseVector2(parts, ref idx, nameof(pivot));
                Vector4 bordersRect = ParseVector4(parts, ref idx, nameof(bordersRect));
                Vector2[] vertices = ParseVertices(parts, ref idx, nameof(vertices));
                UInt16[] triangles = ParseTriangles(parts, ref idx, nameof(triangles));
                SpriteInfo sprite = new SpriteInfo(spriteName, textureRect, pivot, bordersRect, vertices, triangles);
                _sprites.Add(sprite);
            }

            _sprites.TrimExcess();
        }
    }

    private String ParseName(String[] parts, ref Int32 idx, String name)
    {
        ValidateArray(parts, idx, 1, name);

        String value = parts[idx++];

        return value
            .Replace("%23", "#")
            .Replace("%3A", ":")
            .Replace("%3B", ";")
            .Replace("%25", "%")
            .Replace("/", "-");
    }

    private Vector2 ParseVector2(String[] parts, ref Int32 idx, String name)
    {
        ValidateArray(parts, idx, 2, name);

        Int32 startIndex = idx;

        if (!Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
        {
            throw new FormatException($"Failed to parse {name} (x;y) from data: {String.Join(";", parts, startIndex, 2)};");
        }

        return new Vector2(x, y);
    }

    private Rect ParseRect(String[] parts, ref Int32 idx, String name)
    {
        ValidateArray(parts, idx, 4, name);

        Int32 startIndex = idx;

        if (!Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var width)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var height))
        {
            throw new FormatException($"Failed to parse {name} (x;y;width;height) from data: {String.Join(";", parts, startIndex, 4)};");
        }

        return new Rect(x, y, width, height);
    }

    private Vector4 ParseVector4(String[] parts, ref Int32 idx, String name)
    {
        ValidateArray(parts, idx, 4, name);

        Int32 startIndex = idx;

        if (!Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var left)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var right)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var top)
            || !Single.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var bottom))
        {
            throw new FormatException($"Failed to parse {name} (left;right;top;bottom) from data: {String.Join(";", parts, startIndex, 4)};");
        }

        return new Vector4(x: left, y: bottom, z: right, w: top);
    }

    private Vector2[] ParseVertices(String[] parts, ref Int32 idx, String name)
    {
        ValidateArray(parts, idx, 1, name);

        if (!Int32.TryParse(parts[idx++], NumberStyles.Float, CultureInfo.InvariantCulture, out var count))
            throw new FormatException($"Failed to parse [{parts[idx - 1]}] as vertices count. Expected positive integer value.");

        ValidateArray(parts, idx, 2 * count, name);

        Vector2[] vertices = new Vector2[count];
        for (Int32 i = 0; i < count; i++)
            vertices[i] = ParseVector2(parts, ref idx, "vertex");
        return vertices;
    }

    private UInt16[] ParseTriangles(String[] parts, ref Int32 idx, String name)
    {
        ValidateArray(parts, idx, 1, name);

        if (!Int32.TryParse(parts[idx++], NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
            throw new FormatException($"Failed to parse [{parts[idx - 1]}] as triangle count. Expected positive integer value.");

        count *= 3;
        ValidateArray(parts, idx, count, name);

        UInt16[] vertices = new UInt16[count];
        for (Int32 i = 0; i < count; i++)
        {
            if (!UInt16.TryParse(parts[idx++], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                throw new FormatException($"Failed to parse [{parts[idx - 1]}] as triangle point. Expected integer value 0...65535.");

            vertices[i] = value;
        }

        return vertices;
    }

    private void ValidateArray(String[] parts, Int32 index, Int32 length, String name)
    {
        Int32 left = parts.Length - index;
        if (left < length)
            throw new FormatException($"Not enough data ({left} / {length}) to read [{name}] from {index} position of array: {String.Join(";", parts)}");
    }

    private static Boolean TrySkipComments(String line)
    {
        // Skip empty and comment lines
        if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            return true;
        return false;
    }

    private Boolean TryParseHeader(String line)
    {
        if (line[0] == ':')
        {
            Int32 index = line.IndexOf('=');

            // Skip :key or :key=
            if (index < 0 || index >= line.Length - 1)
                return true;

            String key = line.Substring(0, index).Trim();
            String value = line.Substring(index + 1).Trim();

            switch (key)
            {
                case ":format":
                    Format = ParseFormat(value);
                    break;
                case ":texture":
                    TexturePath = ParseTexture(DataPath, value);
                    break;
                case ":size":
                    AtlasSize = ParseSize(value);
                    break;
                case ":pivotpoints":
                    IsPivotsEnabled = ParseEnabled(value);
                    break;
                case ":borders":
                    IsBordersEnabled = ParseEnabled(value);
                    break;
                case ":alphahandling":
                case ":normalmap":
                    // Not Supported
                    return true;
            }

            return true;
        }

        return false;
    }

    private Boolean ParseEnabled(String value)
    {
        return String.Equals(value, "enabled", StringComparison.InvariantCultureIgnoreCase);
    }

    private (Int32 Width, Int32 Height) ParseSize(String value)
    {
        String[] parts = value.Split('x');
        if ((parts.Length != 2)
            || (!Int32.TryParse(parts[0], out var width))
            || (!Int32.TryParse(parts[1], out var height)))
            throw new FormatException($"Failed to parse [{value}] as atlas size.");

        return new(width, height);
    }

    private static String ParseTexture(String sheetPath, String value)
    {
        String directoryName = Path.GetDirectoryName(sheetPath);
        if (directoryName is null)
            throw new ArgumentException($"Cannot get directory of {sheetPath} to find texture.", nameof(sheetPath));

        String texturePath = Path.Combine(directoryName, value);
        if (!File.Exists(texturePath))
            throw new FileNotFoundException($"Texture file [{texturePath}] is not exists.", texturePath);

        return texturePath;
    }

    private static Int32 ParseFormat(String str)
    {
        const Int32 supportedFormat = 40300;

        if (!Int32.TryParse(str, out var value))
            throw new FormatException($"Failed to parse [{str}] as format number. Expected: {supportedFormat} (TexturePacker 6.0.0)");

        if (value != supportedFormat)
            throw new NotSupportedException($"Format {value} of .tpsheet file is not supported. Expected: {supportedFormat} (TexturePacker 6.0.0)");

        return value;
    }
}