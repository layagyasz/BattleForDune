using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UseAbilityEvent(EntityId abilityId, EntityId targetId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UseAbility;
        public bool IsCanceled { get; set; }
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public EntityId InstanceId { get; set; }
    }
}
