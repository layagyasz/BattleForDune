namespace BattleForDune.Core.Events
{
    public enum GameEventType : byte
    {
        AddEffect,
        AddTeam,
        Attack,
        Damage,
        FinalizeInitiative,
        Initiative,
        SpawnUnit,
        Tick,
        UseAbility
    }
}
