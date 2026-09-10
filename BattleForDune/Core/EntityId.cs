namespace BattleForDune.Core
{
    public readonly record struct EntityId(int Value)
    {
        public static readonly EntityId None = new();
    }
}
