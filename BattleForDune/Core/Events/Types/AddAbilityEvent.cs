using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddAbilityEvent(EntityId ownerId, RegistryKey abilityKey) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddAbility;
        public bool IsCanceled { get; set; }
        public EntityId OwnerId { get; set; } = ownerId;
        public RegistryKey AbilityKey { get; set; } = abilityKey;
        public EntityId AbilityId { get; set; }
    }
}
