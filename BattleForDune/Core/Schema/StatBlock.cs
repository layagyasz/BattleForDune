using System.Runtime.CompilerServices;

namespace BattleForDune.Core.Schema
{
    [InlineArray(8)]
    public struct StatBlock
    {
        private Stat _element0;

        public Stat this[StatType stat]
        {
            readonly get => this[(int)stat];
            set => this[(int)stat] = value;
        }
    }
}
