using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddTeamEvent(RegistryKey factionKey) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddTeam;
        public bool IsCanceled { get; set; }
        public EntityId TeamId;
        public RegistryKey FactionKey = factionKey;
    }
}
