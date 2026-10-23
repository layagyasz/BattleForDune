using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RemoveEffectEvent(EntityId subjectId, byte abilityId, EntityId instanceId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.RemoveEffect;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public readonly EntityId TargetId => EntityId.None;
        public EntityId InstanceId = instanceId;
    }
}
