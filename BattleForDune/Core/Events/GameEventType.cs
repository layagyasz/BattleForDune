namespace BattleForDune.Core.Events
{
    public enum GameEventType : byte
    {
        AddAura,
        AddEffect,
        AddUnit,
        Attack,
        Break,
        Damage,
        EnterAura,
        FinalizeInitiative,
        Initiative,
        Kill,
        LeaveAura,
        Move,
        OverrideAllegiance,
        RemoveAura,
        RemoveEffect,
        ResetActions,
        Terminate,
        Tick,
        UpdateActions,
        UpdateStat,
        UseAbility
    }
}
