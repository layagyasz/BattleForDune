using BattleForDune.Core.Schema;
using System.Runtime.CompilerServices;

namespace BattleForDune.Core.Prototypes
{
    [InlineArray(9)]
    public struct StatBlockPrototype
    {
        private Progression _element0;

        public Progression Attack 
        { 
            readonly get => this[StatType.Attack]; 
            set => this[StatType.Attack] = value; 
        }

        public Progression Evasion
        {
            readonly get => this[StatType.Evasion];
            set => this[StatType.Evasion] = value;
        }

        public Progression Armor
        {
            readonly get => this[StatType.Armor];
            set => this[StatType.Armor] = value;
        }

        public Progression Shield
        {
            readonly get => this[StatType.Shield];
            set => this[StatType.Shield] = value;
        }

        public Progression Initiative
        {
            readonly get => this[StatType.Initiative];
            set => this[StatType.Initiative] = value;
        }

        public Progression Structure
        {
            readonly get => this[StatType.Structure];
            set => this[StatType.Structure] = value;
        }

        public Progression Hitpoints
        {
            readonly get => this[StatType.Hitpoints];
            set => this[StatType.Hitpoints] = value;
        }

        public Progression Morale
        {
            readonly get => this[StatType.Morale];
            set => this[StatType.Morale] = value;
        }

        public Progression Threat
        {
            readonly get => this[StatType.Threat];
            set => this[StatType.Threat] = value;
        }

        public Progression this[StatType stat]
        {
            readonly get => this[(int)stat];
            set => this[(int)stat] = value;
        }

        public readonly StatBlock Materialize(byte level)
        {
            var block = new StatBlock();
            for (int i = 0; i < 9; ++i)
            {
                block[i] = ToStat(this[i], level);
            }
            return block;
        }

        private static Stat ToStat(Progression progression, byte level)
        {
            var value = (sbyte)progression.Materialize(level);
            return new(value, 0, value);
        }
    }
}
