using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent(
        EntityId abilityId, EntityId targetId, EntityId instanceId, sbyte skill, sbyte target) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
        public bool IsCanceled { get; set; }
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public EntityId InstanceId { get; set; } = instanceId;
        public sbyte Skill { get; set; } = skill;
        public sbyte Target { get; set; } = target;
        public Die Die0 { get; set; }
        public Die Die1 { get; set; }
        public Die Die2 { get; set; }
        public byte Roll0 { get; set; }
        public byte Roll1 { get; set; }
        public byte Roll2 { get; set; }
    }
}
