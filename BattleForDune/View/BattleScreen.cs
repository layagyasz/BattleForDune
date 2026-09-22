using Cardamom.Graphics;
using Cardamom.Ui;
using OpenTK.Mathematics;

namespace BattleForDune.View
{
    public class BattleScreen(BattleScene scene) : IRenderable
    {
        private readonly BattleScene _scene = scene;

        public void Draw(IRenderTarget target, IUiContext context)
        {
            _scene.Draw(target, context);
        }

        public void Initialize()
        {
            _scene.Initialize();
        }

        public void ResizeContext(Vector3 context)
        {
            _scene.ResizeContext(context);
        }

        public void Update(long delta)
        {
            _scene.Update(delta);
        }
    }
}
