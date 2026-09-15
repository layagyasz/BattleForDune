using BattleForDune.Core.Components.Effects;

namespace BattleForDune.Core.Prototypes
{
    public readonly record struct EffectPrototype(byte Stacks, byte Power, EffectType EffectType);
}
