using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FinalizeInitiativeEvent : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.FinalizeInitiative;
        public bool IsCanceled { get; set; }
    }
}
