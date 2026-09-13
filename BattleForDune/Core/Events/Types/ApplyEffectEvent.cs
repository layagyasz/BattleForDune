using BattleForDune.Core.Components.Effects;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ApplyEffectEvent(
        EntityId subjectId, 
        EntityId abilityId,
        EntityId targetId, 
        EntityId instanceId,
        byte stacks, 
        byte power, 
        byte duration,
        EffectType effectType,
        sbyte skill,
        sbyte difficulty) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.ApplyEffect;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public EntityId InstanceId { get; set; } = instanceId;
        public byte Stacks { get; set; } = stacks;
        public byte Power { get; set; } = power;
        public byte Duration { get; set; } = duration;
        public EffectType EffectType { get; set; } = effectType;
        public sbyte Skill { get; set; } = skill;
        public sbyte Difficulty { get; set; } = difficulty;
        public byte Roll { get; set; }
    }
}
