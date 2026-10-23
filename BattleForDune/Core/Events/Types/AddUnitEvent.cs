using BattleForDune.Geometry;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddUnitEvent(
        EntityId subjectId, 
        byte abilityId,
        RegistryKey unitPrototypeKey,
        byte level,
        RegistryKey factionKey, 
        byte teamId,
        Hex2b position) 
        : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddUnit;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public readonly EntityId TargetId => EntityId.None;
        public RegistryKey UnitPrototypeKey = unitPrototypeKey;
        public byte Level = level;
        public EntityId UnitId;
        public byte TeamId = teamId;
        public RegistryKey FactionKey = factionKey;
        public Hex2b Position = position;
    }
}
