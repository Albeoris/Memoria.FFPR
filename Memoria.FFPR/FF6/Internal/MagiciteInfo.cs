using System;
using Last.Defaine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Memoria.FFPR.FF6.Internal;

[JsonObject(MemberSerialization.OptIn)]
public sealed class MagiciteInfo
{
    [JsonProperty]
    public Int32 Id { get; }
    
    [JsonProperty]
    public Int32 AbilityId { get; }

    [JsonProperty]
    [JsonConverter(typeof(StringEnumConverter))]
    public ParameterType ParameterType { get; }

    [JsonProperty]
    public Int32 ParameterValue { get; }
    
    [JsonProperty]
    public String DescriptionKey { get; }
    
    public String DescriptionMessage { get; }

    public MagiciteInfo(Int32 id, Int32 abilityId, ParameterType parameterType, Int32 parameterValue, String descriptionKey, String descriptionMessage)
    {
        Id = id;
        AbilityId = abilityId;
        ParameterType = parameterType;
        ParameterValue = parameterValue;
        DescriptionKey = descriptionKey;
        DescriptionMessage = descriptionMessage;
    }
}