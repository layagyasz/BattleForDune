using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct RemoveEffectEvent(
        bool IsCanceled, EntityId SubjectId, EntityId AbilityId, EntityId EffectId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.RemoveEffect;
        // TODO: Include owner id so passives can fire?
        public readonly EntityId TargetId => EntityId.None;

        public static RemoveEffectEvent Intransitive(EntityId effectId)
        {
            return new(false, EntityId.None, EntityId.None, effectId);
        }

        public static RemoveEffectEvent Transitive(EntityId subjectId, EntityId abilityId, EntityId effectId)
        {
            return new(false, subjectId, abilityId, effectId);
        }
    }
}
