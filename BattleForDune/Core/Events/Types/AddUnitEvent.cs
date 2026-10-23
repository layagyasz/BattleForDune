using BattleForDune.Geometry;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct AddUnitEvent(
        bool IsCanceled,
        EntityId SubjectId,
        byte AbilityId,
        EntityId TargetId,
        RegistryKey UnitPrototypeKey,
        byte Level,
        RegistryKey FactionKey,
        byte TeamId,
        Hex2b Position) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddUnit;

        public static AddUnitEvent Intransitive(
            RegistryKey unitPrototypeKey,
            byte level,
            RegistryKey factionKey,
            byte teamId,
            Hex2b position)
        {
            return new(
                false,
                EntityId.None,
                0,
                EntityId.None,
                unitPrototypeKey,
                level,
                factionKey,
                teamId,
                position);
        }

        public static AddUnitEvent Transitive(
            EntityId subjectId,
            byte abilityId,
            RegistryKey unitPrototypeKey,
            byte level,
            RegistryKey factionKey,
            byte teamId,
            Hex2b position)
        {
            return new(
                false, 
                subjectId, 
                abilityId,
                EntityId.None,
                unitPrototypeKey,
                level, 
                factionKey,
                teamId, 
                position);
        }
    }
}
