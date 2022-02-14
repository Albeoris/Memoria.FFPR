using Memoria.FFPR.IL2CPP;
using Memoria.FFPR.IL2CPP.HarmonyHooks;

namespace Memoria.FFPR.Core;

public sealed class GameBattleAtbControl : SafeComponent
{
    private readonly HotkeyControl _nextTurnKey = new();
    
    public GameBattleAtbControl()
    {
    }

    protected override void Update()
    {
        ProcessNextRunKey();
    }

    private void ProcessNextRunKey()
    {
        if (!TurnBasedBattleState.IsTurnBased)
            return;
        
        var config = ModComponent.Instance.Config.BattleAtb;
        if (_nextTurnKey.Update(config.NextTurnKey.Value) && _nextTurnKey.IsPressed)
            TurnBasedBattleState.NextTurn();
    }
}