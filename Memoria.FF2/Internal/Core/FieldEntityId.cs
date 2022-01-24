using System;

namespace Memoria.FF2.Internal.Core;

public sealed class FieldEntityId
{
    public Int32 MapId { get; }
    public Int32 EntityId { get; }

    public FieldEntityId(Int32 mapId, Int32 entityId)
    {
        MapId = mapId;
        EntityId = entityId;
    }

    public String AsString()
    {
        return $"({MapId}, {EntityId})";
    }
        
    public override String ToString()
    {
        return AsString();
    }

    public override Boolean Equals(Object obj)
    {
        return ReferenceEquals(this, obj) || obj is FieldEntityId other && Equals(other);
    }

    private Boolean Equals(FieldEntityId other)
    {
        return MapId == other.MapId && EntityId == other.EntityId;
    }

    public override Int32 GetHashCode()
    {
        unchecked
        {
            var hashCode = MapId;
            hashCode = (hashCode * 397) ^ EntityId;
            return hashCode;
        }
    }
}