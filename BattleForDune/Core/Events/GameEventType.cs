namespace BattleForDune.Core.Events
{
    public enum GameEventType : byte
    {
        AddEffect,
        AddUnit,
        Attack,
        Damage,
        FinalizeInitiative,
        Initiative,
        Tick,
        UpdateStat,
        UseAbility
    }
}
