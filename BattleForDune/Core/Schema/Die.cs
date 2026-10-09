namespace BattleForDune.Core.Schema
{
    public struct Die(byte count, byte sides, sbyte modifier)
    {
        public byte Count { get; set; } = count;
        public byte Sides { get; set; } = sides;
        public sbyte Modifier { get; set; } = modifier;

        public readonly byte Roll(Random random)
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
