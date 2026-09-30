using BattleForDune.Core.Components.Abilities.Handlers;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Abilities
{
    public interface IGenericAbilityHandler
    {

        [JsonDerivedType(typeof(UseAbilityAttackHandler.Prototype), "UseAbilityAttack")]
        [JsonDerivedType(typeof(AttackBasicHandler.Prototype), "AttackBasic")]
        [JsonDerivedType(typeof(DamageChanceApplyEffectHandler.Prototype), "DamageChanceApplyEffect")]
        [JsonDerivedType(typeof(DamageCriticalMultiplierHandler.Prototype), "DamageCriticalMultiplier")]
        public interface IHandlerPrototype
        {
            IGenericAbilityHandler Materialize();
        }
    }
}
