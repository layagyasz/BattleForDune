using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AddTeamEvent(EntityId factionId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddTeam;
        public bool IsCanceled { get; set; }
        public EntityId TeamId { get; set; }
        public EntityId FactionId { get; set; } = factionId;
    }
}
