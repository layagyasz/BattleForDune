namespace BattleForDune.Core.Components.Allegiances
{
    public record struct Allegiance
    {
        public RegistryKey Faction;
        public readonly RegistryKey TrueFaction;
        public byte Team;
        public readonly byte TrueTeam;

        public Allegiance(RegistryKey faction, byte team)
        {
            Faction = faction;
            TrueFaction = faction;
            Team = team;
            TrueTeam = team;
        }
    }
}
