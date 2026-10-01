using BattleForDune.Core.Components.Abilities.Handlers;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Abilities
{
    [JsonDerivedType(typeof(UseAbilityAttackHandler), "UseAbilityAttack")]
    [JsonDerivedType(typeof(AttackBasicHandler), "AttackBasic")]
    [JsonDerivedType(typeof(DamageChanceApplyEffectHandler), "DamageChanceApplyEffect")]
    [JsonDerivedType(typeof(DamageCriticalMultiplierHandler), "DamageCriticalMultiplier")]
    public interface IGenericAbilityHandler { }
}
