using BattleForDune.Core.Events;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public readonly record struct GameEventRenderInfo : IComparable<GameEventRenderInfo>
    {
        public required GameEventType EventType { get; init; }
        public required Color4 Color { get; init; }
        public required string Name { get; init; }

        public readonly int CompareTo(GameEventRenderInfo other)
        {
            return EventType.CompareTo(other.EventType);
        }
    }
}
