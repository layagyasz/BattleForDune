using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
    }
}
