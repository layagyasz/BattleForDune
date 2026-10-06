using BattleForDune.Core;
using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using BattleForDune.Json;
using Cardamom.Json;
using Cardamom.Json.Collections;
using Cardamom.Json.OpenTK;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BattleForDune.View.Config
{
    public class ViewConfigurationRegistry
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        static ViewConfigurationRegistry()
        {
            _options.Converters.Add(new ColorJsonConverter());
            _options.Converters.Add(new RegistryKeyJsonConverter());
            _options.Converters.Add(new JsonStringEnumConverter());
        }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<DamageRenderInfo> Damage { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<EffectRenderInfo> Effects { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<GameEventRenderInfo> Events { get; set; }

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required List<TileRenderInfo> Tiles { get; set; }

        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required List<UnitRenderInfo> Units { get; set; }

        public DamageRenderInfo GetDamage(DamageType damageType)
        {
            return Damage[(int)damageType];
        }

        public EffectRenderInfo GetEffect(RegistryKey key)
        {
            return Effects[key.Value];
        }

        public GameEventRenderInfo GetEvent(GameEventType eventType)
        {
            // TODO: Optimize
            foreach (var info in Events)
            {
                if (info.EventType == eventType)
                {
                    return info;
                }
            }
            return default;
        }

        public TileRenderInfo GetTile(RegistryKey key)
        {
            return Tiles[key.Value];
        }

        public UnitRenderInfo GetUnit(RegistryKey key)
        {
            return Units[key.Value];
        }

        public static ViewConfigurationRegistry FromFile(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            var result = JsonSerializer.Deserialize<ViewConfigurationRegistry>(stream, _options)!;
            result.Units.Sort();
            result.Effects.Sort();
            result.Damage.Sort();
            result.Tiles.Sort();
            return result;
        }
    }
}
