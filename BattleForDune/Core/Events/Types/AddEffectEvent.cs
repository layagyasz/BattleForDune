using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct AddEffectEvent(
        bool IsCanceled,
        EntityId SubjectId,
        byte AbilityId,
        EntityId TargetId,
        EntityId InstanceId,
        AbilityVector Vector,
        byte Stacks,
        byte Power,
        sbyte Duration,
        RegistryKey EffectKey,
        sbyte Skill,
        sbyte Difficulty,
        sbyte Roll) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddEffect;

        public static AddEffectEvent Intransitive(
            EntityId targetId,
            AbilityVector vector,
            byte stacks,
            byte power,
            sbyte duration,
            RegistryKey effectKey)
        {
            return new(
                  false,
                  EntityId.None,
                  0,
                  targetId,
                  EntityId.None,
                  vector,
                  stacks,
                  power,
                  duration,
                  effectKey,
                  0,
                  0,
                  0);
        }

        public static AddEffectEvent Transitive(
            EntityId subjectId,
            byte abilityId,
            EntityId targetId,
            AbilityVector vector,
            byte stacks,
            byte power,
            sbyte duration,
            RegistryKey effectKey,
            sbyte skill,
            sbyte difficulty)
        {
            return new(
                  false,
                  subjectId,
                  abilityId,
                  targetId,
                  EntityId.None,
                  vector,
                  stacks,
                  power,
                  duration,
                  effectKey,
                  skill,
                  difficulty,
                  0);
        }
    }
}
