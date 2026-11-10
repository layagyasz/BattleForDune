using BattleForDune.Core.Components.Auras;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;

namespace BattleForDune.Core.Components.Abilities.Targeting
{
    public record struct TileAuraPredicate(BooleanFlag IsPresent, RegistryKey AuraKey)
    {
        private ref struct ContainsAuraCallback(RegistryKey auraKey) : IQuadTreeCallback<Aura>
        {
            private readonly RegistryKey _auraKey = auraKey;
            public bool IsFound { get; private set; }

            public void Notify(Hex2b position, Aura aura)
            {
                IsFound |= aura.AuraKey == _auraKey;
            }
        }

        public readonly bool Matches(Hex2b position, ComponentWorld world)
        {
            if (IsPresent == BooleanFlag.None || IsPresent == BooleanFlag.Any)
            {
                return true;
            }
            var callback = new ContainsAuraCallback(AuraKey);
            world.Auras.QueryRange(position, 0, ref callback);
            return BooleanFlags.HasFlag(IsPresent, callback.IsFound);
        }
    }
}
