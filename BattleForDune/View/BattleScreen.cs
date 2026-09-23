using BattleForDune.Controller;
using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class BattleScreen(IUpdatingController controller, BattleScene scene) : IRenderable
    {
        public IUpdatingController Controller { get; } = controller;
        public BattleScene Scene { get; } = scene;

        public void Draw(IRenderTarget target, IUiContext context)
        {
            Scene.Draw(target, context);
        }

        public void Initialize()
        {
            Controller.Bind(this);
            Scene.Initialize();
        }

        public void ResizeContext(Vector3 context)
        {
            Scene.ResizeContext(context);
        }

        public void Update(long delta)
        {
            Controller.Update(delta);
            Scene.Update(delta);
        }
    }
}
