using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent(EntityId abilityId, byte skill, byte target) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
        public bool IsCanceled { get; set; }
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId InstanceId { get; set; }
        public byte Skill { get; set; } = skill;
        public byte Target { get; set; } = target;
        public byte Roll { get; set; }
    }
}
