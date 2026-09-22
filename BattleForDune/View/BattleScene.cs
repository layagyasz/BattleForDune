using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class BattleScene(ICamera camera, UnitLayer unitLayer) : IRenderable
    {
        private readonly ICamera _camera = camera;
        private readonly UnitLayer _unitLayer = unitLayer;

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(_camera.GetViewMatrix());
            target.PushProjection(_camera.GetProjection());
            _unitLayer.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public void Initialize()
        {
            _unitLayer.Initialize();
        }

        public void ResizeContext(Vector3 context)
        {
            _unitLayer.ResizeContext(context);
        }

        public void Update(long delta)
        {
            _unitLayer.Update(delta);
        }
    }
}
