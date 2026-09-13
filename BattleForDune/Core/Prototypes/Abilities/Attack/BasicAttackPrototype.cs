using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.Attack;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Prototypes.Abilities.Attack
{
    public record class BasicAttackPrototype(
        sbyte Attack, 
        byte CritRange,
        DamageType DamageType,
        Die Damage, 
        sbyte ArmorPiercing,
        sbyte ShieldPiercing) : IHandlerPrototype
    {
        public IGenericHandler Materialize()
        {
            return new BasicAttackHandler(Attack, CritRange, DamageType, Damage, ArmorPiercing, ShieldPiercing);
        }
    }
}
