using BattleForDune.Geometry;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct AddAuraEvent(
        bool IsCanceled,
        EntityId SubjectId,
        EntityId AbilityId,
        EntityId AuraId,
        Hex2b Position,
        RegistryKey AuraKey,
        byte Power,
        byte Range,
        sbyte Duration,
        sbyte Skill,
        sbyte Difficulty,
        sbyte Roll) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddAura;
        public readonly EntityId TargetId => EntityId.None;

        public static AddAuraEvent Transitive(
            EntityId subjectId,
            EntityId abilityId,
            Hex2b position,
            RegistryKey auraKey,
            byte power,
            byte range,
            sbyte duration,
            sbyte skill,
            sbyte difficulty)
        {
            return new(
                false,
                subjectId,
                abilityId,
                EntityId.None,
                position,
                auraKey,
                power,
                range,
                duration,
                skill,
                difficulty,
                0);
        }
    }
}
