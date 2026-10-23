using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UseAbilityEvent(EntityId subjectId, byte abilityId, EntityId targetId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UseAbility;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId { get; set; } = targetId;
    }
}
