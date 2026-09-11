using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.Core.Systems;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var random = new Random();
            var world = new ComponentWorld(
                new((uint)(4 * random.Next() + 1), (uint)(2 * random.Next() + 1), random.Next()));

            var processor = new CombatGameSystem(world);
            var events = new EventPump<EffectComponent, CombatGameSystem>(world.GetRouter(), processor);

            var unitType = 
                new UnitType(
                    new StatBlock(
                        Stat.Of(1),
                        Stat.Of(1),
                        Stat.Of(0), 
                        Stat.Of(0), 
                        Stat.Of(0),
                        Stat.Of(0), 
                        Stat.Of(10),
                        Stat.Of(10),
                        Stat.Of(0)),
                    [AbilityType.Attack]);
            world.AddUnit(unitType, new());

            var unitId = new EntityId(world.GetUnitIds()[0]);
            foreach (var ability in world.GetAbilities(unitId))
            {
                events.Queue(new UseAbilityEvent(ability.Id));
                events.Flush();
            }
        }
    }
}
