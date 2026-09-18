namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(
        RegistryKey Key, 
        LocalizationKey Name,
        StatBlockPrototype Stats,
        RegistryKey[] Passives,
        RegistryKey[] Abilities) 
    { }
}
