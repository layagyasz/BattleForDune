using Cardamom.Graphics;
using Cardamom.Graphics.Camera;
using Cardamom.Mathematics.Geometry;
using Cardamom.Ui;
using Cardamom.Ui.Controller.Element;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class BattleScene(
        IElementController controller,
        ICamera camera, 
        MapLayer mapLayer,
        UnitLayer unitLayer,
        TextParticleLayer textParticleLayer)
        : IRenderable, IInteractive
    {
        public IElementController Controller { get; } = controller;
        public IControlledElement? Parent { get; set; }
        public ICamera Camera { get; } = camera;
        public float? OverrideDepth { get; set; }
        public MapLayer MapLayer { get; } = mapLayer;
        public UnitLayer UnitLayer { get; } = unitLayer;
        public TextParticleLayer TextParticleLayer { get; } = textParticleLayer;

        public void Draw(IRenderTarget target, IUiContext context)
        {
            target.PushViewMatrix(Camera.GetViewMatrix());
            target.PushProjection(Camera.GetProjection());
            context.Register(this);
            MapLayer.Draw(target, context);
            UnitLayer.Draw(target, context);
            TextParticleLayer.Draw(target, context);
            target.PopProjectionMatrix();
            target.PopViewMatrix();
        }

        public float? GetRayIntersection(Ray3 ray)
        {
            return float.MaxValue;
        }

        public void Initialize()
        {
            MapLayer.Initialize();
            UnitLayer.Initialize();
            TextParticleLayer.Initialize();
        }

        public void ResizeContext(Vector3 context)
        {
            Camera.SetAspectRatio(context.X / context.Y);
            MapLayer.ResizeContext(context);
            UnitLayer.ResizeContext(context);
            TextParticleLayer.ResizeContext(context);
        }

        public void Update(long delta)
        {
            MapLayer.Update(delta);
            UnitLayer.Update(delta);
            TextParticleLayer.Update(delta);
        }
    }
}
