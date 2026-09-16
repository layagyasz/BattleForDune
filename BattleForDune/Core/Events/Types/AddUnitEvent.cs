using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddUnitEvent(
        RegistryKey unitPrototypeKey, RegistryKey factionKey, EntityId teamId, Vector2i position) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddUnit;
        public bool IsCanceled { get; set; }
        public RegistryKey UnitPrototypeKey { get; set; } = unitPrototypeKey;
        public EntityId UnitId { get; set; }
        public EntityId TeamId { get; set; } = teamId;
        public RegistryKey FactionKey { get; set; } = factionKey;
        public Vector2i Position { get; set; } = position;
    }
}
