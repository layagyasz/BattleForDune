using BattleForDune.Core.Components.Abilities.Handlers;
using System.Text.Json.Serialization;

namespace BattleForDune.Core.Components.Abilities
{

    [JsonDerivedType(typeof(AttackBasicHandler), "AttackBasic")]
    [JsonDerivedType(typeof(AttackChanceAddAuraHandler), "AttackChanceAddAura")]
    [JsonDerivedType(typeof(AttackChanceAddEffectHandler), "AttackChanceAddEffect")]
    [JsonDerivedType(typeof(DamageChanceAddEffectHandler), "DamageChanceAddEffect")]
    [JsonDerivedType(typeof(DamageCriticalMultiplierHandler), "DamageCriticalMultiplier")]
    [JsonDerivedType(typeof(UseAbilityAttackHandler), "UseAbilityAttack")]
    [JsonDerivedType(typeof(UseAbilityAuraHandler), "UseAbilityAura")]
    public interface IGenericAbilityHandler { }
}
