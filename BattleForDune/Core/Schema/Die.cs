namespace BattleForDune.Core.Schema
{
    public struct Die(byte count, byte sides, sbyte modifier, sbyte advantage)
    {
        public byte Count { get; set; } = count;
        public byte Sides { get; set; } = sides;
        public sbyte Modifier { get; set; } = modifier;
        public sbyte Advantage { get; set; } = advantage;

        public readonly byte Roll(Random random)
        {
            byte result = RollInternal(random);
            if (Advantage < 0)
            {
                return Math.Min(result, RollInternal(random));
            }
            if (Advantage > 0)
            {
                return Math.Max(result, RollInternal(random));
            }
            return result;
        }

        private readonly byte RollInternal(Random random)
        {
            int result = Modifier + Count;
            for (int i = 0; i < Count; ++i)
            {
                result += random.Next(Sides);
            }
            return (byte)Math.Max(result, 0);
        }
    }
}
