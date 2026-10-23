using BattleForDune.Geometry;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct MoveEvent(
        bool IsCanceled, EntityId SubjectId, Hex2b Origin, Hex2b Destination) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Move;
        public readonly EntityId TargetId => EntityId.None;

        public static MoveEvent Intransitive(EntityId subjectId, Hex2b origin, Hex2b destination)
        {
            return new(false, subjectId, origin, destination);
        }
    }
}
