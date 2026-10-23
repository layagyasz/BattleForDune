using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct KillEvent(
        bool IsCanceled,
        EntityId SubjectId, 
        byte AbilityId, 
        EntityId TargetId, 
        bool IsPrimaryDamage) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Kill;

        public static KillEvent Transitive(
            EntityId subjectId, byte abilityId, EntityId targetId, bool isPrimaryDamage)
        {
            return new(false, subjectId, abilityId, targetId, isPrimaryDamage);
        }
    }
}
