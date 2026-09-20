using BattleForDune.Geometry;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddUnitEvent(
        RegistryKey unitPrototypeKey, RegistryKey factionKey, byte teamId, Hex2b position) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddUnit;
        public bool IsCanceled { get; set; }
        public RegistryKey UnitPrototypeKey = unitPrototypeKey;
        public EntityId UnitId;
        public byte TeamId = teamId;
        public RegistryKey FactionKey = factionKey;
        public Hex2b Position = position;
    }
}
