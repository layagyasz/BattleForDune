using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent(
        EntityId subjectId, EntityId abilityId, EntityId targetId, EntityId instanceId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
        public bool IsCanceled { get; set; }
        public bool IsCritical { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public EntityId InstanceId { get; set; } = instanceId;
        public byte Roll { get; set; }
    }
}
