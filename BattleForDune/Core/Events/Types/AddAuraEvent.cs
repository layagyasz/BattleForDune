using BattleForDune.Core.Schema;
using BattleForDune.Geometry;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct AddAuraEvent(
        bool IsCanceled,
        EntityId SubjectId,
        byte AbilityId,
        EntityId AuraId,
        Hex2b Position,
        AbilityVector Vector,
        RegistryKey EffectKey,
        byte Power,
        byte Range,
        sbyte Duration) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddAura;
        public readonly EntityId TargetId => EntityId.None;

        public static AddAuraEvent Transitive(
            EntityId subjectId,
            byte abilityId,
            Hex2b position,
            AbilityVector vector,
            RegistryKey effectKey,
            byte power,
            byte range,
            sbyte duration)
        {
            return new(
                false,
                subjectId,
                abilityId,
                EntityId.None,
                position,
                vector,
                effectKey,
                power,
                range,
                duration);
        }
    }
}
