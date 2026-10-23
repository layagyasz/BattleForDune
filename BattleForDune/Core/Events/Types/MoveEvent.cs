using BattleForDune.Geometry;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MoveEvent(EntityId subjectId, Hex2b origin, Hex2b destination) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Move;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public readonly EntityId TargetId => EntityId.None;
        public Hex2b Origin = origin;
        public Hex2b Destination = destination;
    }
}
