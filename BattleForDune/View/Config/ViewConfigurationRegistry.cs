using BattleForDune.Core;
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
        [JsonConverter(typeof(FromMultipleFileJsonConverter))]
        public required List<UnitRenderInfo> Units { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<EffectRenderInfo> Effects { get; set; }

        [JsonConverter(typeof(FromFileJsonConverter))]
        public required List<DamageRenderInfo> Damage { get; set; }

        public DamageRenderInfo GetDamage(DamageType damageType)
        {
            return Damage[(int)damageType];
        }

        public EffectRenderInfo GetEffect(RegistryKey key)
        {
            return Effects[key.Value];
        }

        public UnitRenderInfo GetUnit(RegistryKey key)
        {
            return Units[key.Value];
        }

        public static ViewConfigurationRegistry FromFile(string path)
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ColorJsonConverter());
            options.Converters.Add(new RegistryKeyJsonConverter());
            using var stream = new FileStream(path, FileMode.Open);
            var result = JsonSerializer.Deserialize<ViewConfigurationRegistry>(stream, options)!;
            result.Units.Sort();
            result.Effects.Sort();
            result.Damage.Sort();
            return result;
        }
    }
}
