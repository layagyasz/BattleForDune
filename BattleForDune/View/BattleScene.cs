using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class BattleScene(
        IElementController controller, ICamera camera, UnitLayer unitLayer) : IRenderable, IInteractive
    {
        public IElementController Controller { get; } = controller;
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; } = camera;
        public float? OverrideDepth { get; set; }
        public UnitLayer UnitLayer { get; } = unitLayer;

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            UnitLayer.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            UnitLayer.Initialize();
        }

        public void ResizeContext(Vector3 context)
        {
            Camera.SetAspectRatio(context.X / context.Y);
            UnitLayer.ResizeContext(context);
        }

        public void Update(long delta)
        {
            UnitLayer.Update(delta);
        }
    }
}
