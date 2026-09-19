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
        public RegistryKey UnitPrototypeKey = unitPrototypeKey;
        public EntityId UnitId;
        public EntityId TeamId = teamId;
        public RegistryKey FactionKey = factionKey;
        public Vector2i Position = position;
    }
}
