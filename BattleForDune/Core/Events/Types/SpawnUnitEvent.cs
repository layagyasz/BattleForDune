using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SpawnUnitEvent(EntityId unitPrototypeId, EntityId teamId, Vector2i position) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.SpawnUnit;
        public bool IsCanceled { get; set; }
        public EntityId UnitId { get; set; }
        public EntityId UnitPrototypeId { get; set; } = unitPrototypeId;
        public EntityId TeamId { get; set; } = teamId;
        public Vector2i Position { get; set; } = position;
    }
}
