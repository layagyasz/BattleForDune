using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddEffectEvent(
        EntityId subjectId, 
        byte abilityId,
        EntityId targetId, 
        byte stacks, 
        byte power, 
        sbyte duration,
        RegistryKey effectKey,
        sbyte skill,
        sbyte difficulty) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddEffect;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public EntityId InstanceId;
        public byte Stacks = stacks;
        public byte Power = power;
        public sbyte Duration = duration;
        public RegistryKey EffectKey = effectKey;
        public sbyte Skill = skill;
        public sbyte Difficulty = difficulty;
        public byte Roll;
    }
}
