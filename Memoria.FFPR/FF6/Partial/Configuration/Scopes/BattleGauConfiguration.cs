using System;
using BepInEx.Configuration;
using UnityEngine;
using Object = System.Object;

namespace Memoria.FFPR.Configuration.Scopes;

[ConfigScope("Battle.Gau")]
public abstract partial class BattleGauConfiguration
{
    [ConfigEntry($"Replace empty descriptions of rage abilities with ones generated based on data from game resources.")]
    public virtual Boolean GenerateRageAbilityDescription => true;
    
    [ConfigEntry($"Rage abilities sort order.")]
    public virtual AbilityOrder RageAbilitiesSortOrder => AbilityOrder.Alphabetical;

    [ConfigEntry($"Colorize Leap, in battle with unexplored opponents.")]
    public virtual Boolean ColorizeLeapCommand => true;

    [ConfigEntry($"The color of the Leap command in battle with unexplored opponents.")]
    [ConfigConverter(nameof(LeapCommandSensibleColorConverter))]
    public virtual Color LeapCommandSensibleColor => Color.cyan;

    [ConfigEntry($"The color of the Leap command in battle when all abilities are learned.")]
    [ConfigConverter(nameof(LeapCommandSenselessColorConverter))]
    public virtual Color LeapCommandSenselessColor => Color.grey;
    
    [ConfigEntry($"Display unacquired rage abilities.")]
    public virtual Boolean DisplayUnacquiredRageAbilities => false;

    [ConfigEntry($"Color of unacquired rage abilities.")]
    [ConfigConverter(nameof(UnacquiredAbilitiesColorConverter))]
    public virtual Color UnacquiredRageAbilitiesColor => new Color32(255, 0, 0, 100);

    public abstract void CopyFrom(BattleGauConfiguration configuration);

    protected IAcceptableValue<Color> UnacquiredAbilitiesColorConverter { get; } = new AcceptableColor(nameof(UnacquiredRageAbilitiesColor));
    protected IAcceptableValue<Color> LeapCommandSensibleColorConverter { get; } = new AcceptableColor(nameof(LeapCommandSensibleColor));
    protected IAcceptableValue<Color> LeapCommandSenselessColorConverter { get; } = new AcceptableColor(nameof(LeapCommandSenselessColor));
}