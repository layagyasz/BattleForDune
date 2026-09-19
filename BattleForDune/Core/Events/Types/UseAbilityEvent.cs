using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UseAbilityEvent(EntityId subjectId, EntityId abilityId, EntityId targetId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UseAbility;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId = subjectId;
        public EntityId AbilityId = abilityId;
        public EntityId TargetId = targetId;
    }
}
