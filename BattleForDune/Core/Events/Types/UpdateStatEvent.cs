using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct UpdateStatEvent(
        bool IsCanceled,
        EntityId SubjectId,
        byte AbilityId,
        EntityId TargetId,
        StatType Stat,
        Stat Delta,
        bool IsReversal) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UpdateStat;

        public static UpdateStatEvent Transitive(
            EntityId subjectId, byte abilityId, EntityId targetId, StatType stat, Stat delta)
        {
            return new(false, subjectId, abilityId, targetId, stat, delta, false);
        }

        public static UpdateStatEvent TransitiveReversal(
            EntityId subjectId, byte abilityId, EntityId targetId, StatType stat, Stat delta)
        {
            return new(false, subjectId, abilityId, targetId, stat, delta, true);
        }
    }
}
