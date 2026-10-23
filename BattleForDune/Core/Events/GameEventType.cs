namespace BattleForDune.Core.Events
{
    public enum GameEventType : byte
    {
        AddEffect,
        AddUnit,
        Attack,
        Break,
        Damage,
        FinalizeInitiative,
        Initiative,
        Kill,
        Move,
        OverrideAllegiance,
        RemoveEffect,
        ResetActions,
        Terminate,
        Tick,
        UpdateActions,
        UpdateStat,
        UseAbility
    }
}
