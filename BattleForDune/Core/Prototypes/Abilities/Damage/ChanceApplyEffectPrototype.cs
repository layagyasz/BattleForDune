using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.Damage;
using BattleForDune.Core.Components.Effects;
namespace BattleForDune.Core.Prototypes.Abilities.Damage
{
    public record class ChanceApplyEffectPrototype(EffectType EffectType, sbyte Difficulty, byte Duration)
        : IHandlerPrototype
    {
        public IGenericHandler Materialize()
        {
            return new ChanceApplyEffectHandler(EffectType, Difficulty, Duration);
        }
    }
}
