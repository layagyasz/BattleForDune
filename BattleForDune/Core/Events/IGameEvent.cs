namespace BattleForDune.Core.Events
{
    public interface IGameEvent
    {
        GameEventType Type { get; }
        bool IsCanceled { get; }
    }
}
