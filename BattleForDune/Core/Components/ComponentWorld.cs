using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;
using Cardamom.Utils.Suppliers.Ids;
using OpenTK.Mathematics;
using System.Numerics;

namespace BattleForDune.Core.Components
{
    public class ComponentWorld(
        LinearCongruentialIdSupplier ids, ConfigurationRegistry registry, DenseHexGrid<Tile> map, Random random) 
        : IGameEventRouter
    {
        public enum WorldState : byte
        {
            Running,
            Terminated
        }

        private readonly LinearCongruentialIdSupplier _ids = ids;

        public ConfigurationRegistry Registry { get; init; } = registry;
        public DenseHexGrid<Tile> Map { get; init; } = map;
        public Random Random { get; init; } = random;

        public byte Turn { get; private set; }
        public WorldState State { get; private set; }
        public IdSet Units { get; init; } = new(1024);
        public AllegianceComponent Allegiances { get; init; } = new(1024);
        public AuraComponent Auras { get; init; } = new(Upsize(map.Size), 1024);
        public PositionComponent Positions { get; init; } = new(Upsize(map.Size), 1024);
        public ActionComponent Actions { get; init; } = new(1024);
        public BehaviorComponent Behaviors { get; init; } = new(1024);
        public UnitMetadataComponent UnitMetadatas { get; init; } = new(1024);
        public StatComponent Stats { get; init; } = new(1024);
        public InitiativeComponent Inititiative { get; init; } = new(1024);
        public EffectComponent Effects { get; init; } = new(registry, 4 * 1024);
        public AbilityComponent Abilities { get; init; } = new(1024);
        public TimerComponent Timers { get; init; } = new(1024);

        public EntityId NextId()
        {
            var id = _ids.Get();
            if (id == 0)
            {
                id = _ids.Get();
            }
            return new(id);
        }

        public byte IncrementTurn()
        {
            return ++Turn;
        }

        public sbyte Roll(Die die)
        {
            return die.Roll(Random);
        }

        public void Terminate()
        {
            State = WorldState.Terminated;
        }

        public void Intercept<T>(ref T @event) where T : struct, IGameEvent
        {
            if (@event.SubjectId != EntityId.None)
            {
                foreach (var effect in Effects.ByOwner(@event.SubjectId))
                {
                    if (!effect.IsGlobal)
                    {
                        effect.Intercept(ref @event, this);
                    }
                }
            }
            if (@event.TargetId != EntityId.None && @event.TargetId != @event.SubjectId)
            {
                foreach (var effect in Effects.ByOwner(@event.TargetId))
                {
                    if (!effect.IsGlobal)
                    {
                        effect.Intercept(ref @event, this);
                    }
                }
            }
            foreach (var effect in Effects.ByEventType(@event.Type))
            {
                effect.Intercept(ref @event, this);
            }
        }

        public void Handle<T>(in T @event, IGameEventSink sink) where T : struct, IGameEvent
        {
            if (@event.SubjectId != EntityId.None)
            {
                foreach (var effect in Effects.ByOwner(@event.SubjectId))
                {
                    if (!effect.IsGlobal)
                    {
                        effect.Handle(in @event, this, sink);
                    }
                }
            }
            if (@event.TargetId != EntityId.None && @event.TargetId != @event.SubjectId)
            {
                foreach (var effect in Effects.ByOwner(@event.TargetId))
                {
                    if (!effect.IsGlobal)
                    {
                        effect.Handle(in @event, this, sink);
                    }
                }
            }
            foreach (var effect in Effects.ByEventType(@event.Type))
            {
                effect.Handle(in @event, this, sink);
            }
        }

        private static HexRect2b Upsize(Vector2i size)
        {
            byte width = (byte)BitOperations.RoundUpToPowerOf2((uint)size.X);
            byte height = (byte)BitOperations.RoundUpToPowerOf2((uint)size.Y);
            return new HexRect2b(new(0, -height / 2), new(width, 3 * height / 2));
        }
    }
}
