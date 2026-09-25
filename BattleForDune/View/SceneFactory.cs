using BattleForDune.Controller;
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
        private readonly MapLayer.Factory _mapLayerFactory = new(gridShader);
        private readonly UnitLayer.Factory _unitLayerFactory = new(shader);
        private readonly TextParticleLayer.Factory _textParticleLayerFactory = new(font, textParticleShader);

        public BattleScene CreateBattle(int capacity)
        {
            var camera = new SubjectiveCamera3d(1000);
            camera.SetPitch(-0.5f * MathF.PI);
            camera.SetYaw(0.5f * MathF.PI);
            camera.SetDistance(100);

            var textures = new UnitTextureCache(registry, font, shader, 32);
            textures.AddAll([new(0)]);

            return new(
                new PassthroughController(
                    new Camera2dController(camera)
                    {
                        KeySensitivity = 0.0005f,
                        DistanceRange = new(5, 1000),
                        MouseWheelSensitivity = 2
                    }),
                camera,
                _mapLayerFactory.Create(new(100, 100)),
                _unitLayerFactory.Create(textures, capacity),
                _textParticleLayerFactory.Create(4096));
        }
    }
}