namespace BattleForDune.Core.Schema
{
    public readonly record struct Die(byte Count, byte Sides, sbyte Modifier)
    {
        public Die WithModifier(sbyte modifier)
        {
            return new(Count, Sides, (sbyte)(Modifier + modifier));
        }

        public byte Roll(Random random)
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
