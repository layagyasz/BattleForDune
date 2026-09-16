namespace BattleForDune.Core.Events
{
    public enum GameEventType : byte
    {
        AddAbility,
        AddEffect,
        AddTeam,
        AddUnit,
        Attack,
        Damage,
        FinalizeInitiative,
        Initiative,
        Tick,
        UseAbility
    }
}
