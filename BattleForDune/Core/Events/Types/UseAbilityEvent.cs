using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct UseAbilityEvent(
        bool IsCanceled, EntityId SubjectId, byte AbilityId, EntityId TargetId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UseAbility;

        public static UseAbilityEvent Transitive(EntityId subjectId, byte abilityId, EntityId targetId)
        {
            return new(false, subjectId, abilityId, targetId);
        }
    }
}
