using System.Runtime.InteropServices;

namespace BattleForDune.Core.Schema
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StatBlock(
        Stat attack,
        Stat defense,
        Stat armor,
        Stat shield,
        Stat initiative,
        Stat hitpoints,
        Stat morale,
        Stat threat)
    {
        public Stat Attack = attack;
        public Stat Defense = defense;
        public Stat Armor = armor;
        public Stat Shield = shield;
        public Stat Initiative = initiative;
        public Stat Hitpoints = hitpoints;
        public Stat Morale = morale;
        public Stat Threat = threat;

        public void Add(in StatBlock other)
        {
            Attack.Add(other.Attack);
            Defense.Add(other.Defense);
            Armor.Add(other.Armor);
            Shield.Add(other.Shield);
            Initiative.Add(other.Initiative);
            Hitpoints.Add(other.Hitpoints);
            Morale.Add(other.Morale);
            Threat.Add(other.Threat);
        }

        public void Subtract(in StatBlock other)
        {
            Attack.Subtract(other.Attack);
            Defense.Subtract(other.Defense);
            Armor.Subtract(other.Armor);
            Shield.Subtract(other.Shield);
            Initiative.Subtract(other.Initiative);
            Hitpoints.Subtract(other.Hitpoints);
            Morale.Subtract(other.Morale);
            Threat.Subtract(other.Threat);
        }
    }
}
