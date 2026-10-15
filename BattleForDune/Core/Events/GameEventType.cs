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
        RemoveEffect,
        Terminate,
        Tick,
        UpdateStat,
        UseAbility
    }
}
