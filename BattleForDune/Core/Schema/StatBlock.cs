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

        public void Combine(in StatBlock other)
        {
            Attack.Combine(other.Attack);
            Defense.Combine(other.Defense);
            Armor.Combine(other.Armor);
            Shield.Combine(other.Shield);
            Initiative.Combine(other.Initiative);
            Hitpoints.Combine(other.Hitpoints);
            Morale.Combine(other.Morale);
            Threat.Combine(other.Threat);
        }

        public void Remove(in StatBlock other)
        {
            Attack.Remove(other.Attack);
            Defense.Remove(other.Defense);
            Armor.Remove(other.Armor);
            Shield.Remove(other.Shield);
            Initiative.Remove(other.Initiative);
            Hitpoints.Remove(other.Hitpoints);
            Morale.Remove(other.Morale);
            Threat.Remove(other.Threat);
        }
    }
}
