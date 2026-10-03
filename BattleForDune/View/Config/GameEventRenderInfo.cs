using BattleForDune.Core.Events;
using OpenTK.Mathematics;

namespace BattleForDune.View.Config
{
    public record struct GameEventRenderInfo : IComparable<GameEventRenderInfo>
    {
        public required GameEventType EventType { get; set; }
        public required Color4 Color { get; set; }
        public required string Name { get; set; }

        public readonly int CompareTo(GameEventRenderInfo other)
        {
            return EventType.CompareTo(other.EventType);
        }
    }
}
