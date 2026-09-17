using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddEffectEvent(
        EntityId subjectId, 
        EntityId abilityId,
        EntityId targetId, 
        byte stacks, 
        byte power, 
        byte duration,
        RegistryKey effectKey,
        sbyte skill,
        sbyte difficulty) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddEffect;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public EntityId InstanceId { get; set; }
        public byte Stacks { get; set; } = stacks;
        public byte Power { get; set; } = power;
        public byte Duration { get; set; } = duration;
        public RegistryKey EffectKey { get; set; } = effectKey;
        public sbyte Skill { get; set; } = skill;
        public sbyte Difficulty { get; set; } = difficulty;
        public byte Roll { get; set; }
    }
}
