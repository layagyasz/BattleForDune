using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Schema;
using BattleForDune.Json;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Json.Mathematics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.Core
{
    public record class ConfigurationRegistry
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            IncludeFields = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        static ConfigurationRegistry()
        {
            _options.Converters.Add(new DieJsonConverter());
            _options.Converters.Add(new EnumSetJsonConverter());
            _options.Converters.Add(new FlagJsonConverter());
            _options.Converters.Add(new IntervalJsonConverter());
            _options.Converters.Add(new RegistryKeyJsonConverter());
            _options.Converters.Add(new JsonStringEnumConverter());
        }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<Ability.Prototype> Abilities { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<Effect.Prototype> Effects { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<Faction> Factions { get; set; }

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required List<UnitPrototype> Units { get; set; }

        public static ConfigurationRegistry FromFile(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            var result = JsonSerializer.Deserialize<ConfigurationRegistry>(stream, _options)!;
            result.Abilities.Sort();
            result.Effects.Sort();
            result.Factions.Sort();
            result.Units.Sort();
            return result;
        }

        public Ability.Prototype GetAbilityPrototype(RegistryKey key)
        {
            return Abilities[key.Value];
        }

        public Effect.Prototype GetEffectPrototype(RegistryKey key)
        {
            return Effects[key.Value];
        }

        public Faction GetFaction(RegistryKey key)
        {
            return Factions[key.Value];
        }

        public UnitPrototype GetUnitPrototype(RegistryKey key)
        {
            return Units[key.Value];
        }
    }
}
