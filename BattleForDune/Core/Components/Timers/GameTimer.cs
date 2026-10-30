namespace BattleForDune.Core.Components.Timers
{
    public record struct GameTimer(EntityId OwnerId, GameTimerType Type, byte Turn);
}
