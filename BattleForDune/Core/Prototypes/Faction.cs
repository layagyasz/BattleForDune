namespace BattleForDune.Core.Prototypes
{
    public record class Faction(EntityId Id, LocalizationKey Name, EffectPrototype[] Effects);
}
