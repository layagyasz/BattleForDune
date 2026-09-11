using System.Runtime.InteropServices;

namespace BattleForDune.Core.Schema
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct StatBlock(
        Stat Attack,
        Stat Defense,
        Stat Armor,
        Stat Shield,
        Stat Damage,
        Stat Initiative,
        Stat Hitpoints,
        Stat Morale,
        Stat Threat)
    {
        public void Add(in StatBlock other)
        {
            Attack.Add(other.Attack);
            Defense.Add(other.Defense);
            Armor.Add(other.Armor);
            Shield.Add(other.Shield);
            Damage.Add(other.Damage);
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
            Damage.Subtract(other.Damage);
            Initiative.Subtract(other.Initiative);
            Hitpoints.Subtract(other.Hitpoints);
            Morale.Subtract(other.Morale);
            Threat.Subtract(other.Threat);
        }
    }
}
