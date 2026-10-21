using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class ResetActionsHandler(bool IsBlockAttacks, bool IsBlockMoves) 
        : IEffectHandler<ResetActionsEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref ResetActionsEvent @event, ComponentWorld world)
        {
            if (@event.SubjectId != parent.OwnerId)
            {
                return;
            }
            @event.IsCanceled = IsBlockAttacks && IsBlockMoves;
            if (IsBlockAttacks)
            {
                @event.Attacks = 0;
            }
            if (IsBlockMoves)
            {
                @event.Moves = 0;
            }
        }

        public void Handle<TSink>(
            Effect parent, in ResetActionsEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
