using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct RemoveEffectEvent(
        bool IsCanceled, EntityId SubjectId, byte AbilityId, EntityId InstanceId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.RemoveEffect;
        // TODO: Include owner id so passives can fire?
        public readonly EntityId TargetId => EntityId.None;

        public static RemoveEffectEvent Intransitive(EntityId instanceId)
        {
            return new(false, EntityId.None, 0, instanceId);
        }

        public static RemoveEffectEvent Transitive(EntityId subjectId, byte abilityId, EntityId instanceId)
        {
            return new(false, subjectId, abilityId, instanceId);
        }
    }
}
