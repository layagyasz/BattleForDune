
using System.Runtime.InteropServices;

namespace BattleForDune.Schema
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct StatBlock(
        sbyte Attack, 
        sbyte Defense,
        sbyte Armor, 
        sbyte Shield, 
        sbyte Damage,
        sbyte Initiative,
        sbyte Hitpoints,
        sbyte Morale)
    {
        public void Add(StatBlock other)
        {
            Attack += other.Attack;
            Defense += other.Defense;
            Armor += other.Armor;
            Shield += other.Shield;
            Damage += other.Damage;
            Initiative += other.Initiative;
            Hitpoints += other.Hitpoints;
            Morale += other.Morale;
        }

        public void Subtract(StatBlock other)
        {
            Attack -= other.Attack;
            Defense -= other.Defense;
            Armor -= other.Armor;
            Shield -= other.Shield;
            Damage -= other.Damage;
            Initiative -= other.Initiative;
            Hitpoints -= other.Hitpoints;
            Morale -= other.Morale;
        }
    }
}
