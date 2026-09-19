using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddAbilityEvent(EntityId ownerId, RegistryKey abilityKey) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddAbility;
        public bool IsCanceled { get; set; }
        public EntityId OwnerId = ownerId;
        public RegistryKey AbilityKey = abilityKey;
        public EntityId AbilityId;
    }
}
