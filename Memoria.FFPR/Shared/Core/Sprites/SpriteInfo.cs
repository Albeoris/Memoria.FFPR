using System;
using System.Linq;
using Il2CppSystem.Input;
using Memoria.FFPR.BeepInEx;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace Memoria.FFPR.Core;

public sealed class SpriteInfo
{
  public String Name { get; }
  public Rect TextureRect { get; }
  public Vector2 Pivot { get; }
  public Vector4 BordersRect { get; }
  public Vector2[] Vertices { get; }
  public UInt16[] Triangles { get; }

  public SpriteInfo(String name, Rect textureRect, Vector2 pivot, Vector4 bordersRect, Vector2[] vertices, UInt16[] triangles)
  {
    Name = name;
    TextureRect = textureRect;
    Pivot = pivot;
    BordersRect = bordersRect;
    Vertices = vertices;
    Triangles = triangles;
  }

  public Il2CppStructArray<Vector2> ScaleVertices(Vector2 scale)
  {
    if (scale == Vector2.one)
      return Vertices.ToStructArray();

    return Vertices
      .Select(v => v * scale)
      .ToArray()
      .ToStructArray();
  }
  
  public Il2CppStructArray<Vector2> ScaleVertices(Int32 scale)
  {
    if (scale == 1)
      return Vertices.ToStructArray();

    return Vertices
      .Select(v => v * scale)
      .ToArray()
      .ToStructArray();
  }
}