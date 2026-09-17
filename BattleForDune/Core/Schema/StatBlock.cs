using System.Runtime.CompilerServices;

namespace BattleForDune.Core.Schema
{
    [InlineArray(8)]
    public struct StatBlock
    {
        private Stat _element0;

        public readonly sbyte Get(StatType stat)
        {
            return this[(int)stat].Value;
        }

        public void Set(StatType stat, Stat value)
        {
            this[(int)stat] = value;
        }
    }
}
