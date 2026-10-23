using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UpdateStatEvent(
        EntityId subjectId,
        byte abilityId,
        EntityId targetId,
        StatType stat,
        Stat delta,
        bool isReversal) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UpdateStat;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public StatType Stat = stat;
        public Stat Delta = delta;
        public bool IsReversal = isReversal;
    }
}
