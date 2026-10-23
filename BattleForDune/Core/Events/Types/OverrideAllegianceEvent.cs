using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct OverrideAllegianceEvent(
        bool IsCanceled,
        EntityId SubjectId,
        byte AbilityId,
        EntityId TargetId,
        RegistryKey FactionKey,
        byte TeamId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.OverrideAllegiance;

        public static OverrideAllegianceEvent Transitive(
            EntityId subjectId, byte abilityId, EntityId targetId, RegistryKey factionKey, byte teamId)
        {
            return new(false, subjectId, abilityId, targetId, factionKey, teamId);
        }
    }
}
