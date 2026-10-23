namespace BattleForDune.Core.Components.Effects
{
    [Flags]
    public enum MergeBehavior : byte
    {
        None = 0,
        Instance = 1,
        Timer = 2,
        Stacks = 4,
        Power = 8,
        All = 255
    }
}
