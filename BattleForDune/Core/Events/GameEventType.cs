namespace BattleForDune.Core.Events
{
    public enum GameEventType : byte
    {
        ApplyEffect,
        Attack,
        Damage,
        FinalizeInitiative,
        Initiative,
        SpawnUnit,
        Tick,
        UseAbility
    }
}
