using Cardamom.Ui.Controller;

namespace BattleForDune.Controller
{
    public interface IUpdatingController : IController
    {
        void Update(long delta);
    }
}
