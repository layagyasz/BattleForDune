namespace BattleForDune.Core.Schema
{
    public readonly record struct Die(byte Count, byte Sides, sbyte Modifier, sbyte Advantage)
    {
        public byte Roll(Random random)
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

        private byte RollInternal(Random random)
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
