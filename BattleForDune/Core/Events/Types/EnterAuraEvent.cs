using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct EnterAuraEvent(EntityId SubjectId, EntityId AuraId, EntityId EffectId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.EnterAura;
        public readonly bool IsCanceled => false;
        public readonly EntityId TargetId => EntityId.None;

        public static EnterAuraEvent Intransitive(EntityId subjectId, EntityId auraId)
        {
            return new(subjectId, auraId, EntityId.None);
        }
    }
}
