using BattleForDune.Controller;
using BattleForDune.Core.Schema;
using BattleForDune.Geometry;
using BattleForDune.View.Config;
using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Ui.Controller.Element;

namespace BattleForDune.View
{
    public class SceneFactory(
        ViewConfigurationRegistry registry, 
        RenderShader shader,
        RenderShader gridShader,
        RenderShader textParticleShader, 
        Font font)
    {
        private readonly MapLayer.Factory _mapLayerFactory = new(shader, gridShader);
        private readonly AuraLayer.Factory _auraLayerFactory = new(shader);
        private readonly UnitLayer.Factory _unitLayerFactory = new(shader);
        private readonly TextParticleLayer.Factory _textParticleLayerFactory = new(font, textParticleShader);

        public BattleScene CreateBattle(DenseHexGrid<Tile> map, int capacity)
        {
            var camera = new SubjectiveCamera3d(1000);
            camera.SetFocus(
                Geometry.Projection.AxialToCartesian(Geometry.Projection.OffsetToAxial(map.Size / 2)));
            camera.SetPitch(-0.5f * MathF.PI);
            camera.SetYaw(0.5f * MathF.PI);
            camera.SetDistance(10);

            var unitTextures = new UnitTextureCache(registry, font, shader, 32);
            unitTextures.AddAll(registry.Units.Select(unit => unit.Key));

            var tileTextures = new TileTextureCache(32);
            tileTextures.AddAll(
                map.Values()
                    .Select(tile => tile.TileKey)
                    .Distinct()
                    .Select(key => (key, registry.GetTile(key).ImagePath)));

            var auraTextures = new TileTextureCache(32);
            auraTextures.AddAll(
                registry.Auras.Select(aura => (aura.Key, registry.GetAura(aura.Key).ImagePath)));

            return new(
                new PassthroughController(
                    new Camera2dController(camera)
                    {
                        KeySensitivity = 0.0005f,
                        DistanceRange = new(5, 1000),
                        MouseWheelSensitivity = 2
                    }),
                camera,
                _mapLayerFactory.Create(tileTextures, map),
                _auraLayerFactory.Create(auraTextures, capacity),
                _unitLayerFactory.Create(unitTextures, capacity),
                _textParticleLayerFactory.Create(8192));
        }
    }
}