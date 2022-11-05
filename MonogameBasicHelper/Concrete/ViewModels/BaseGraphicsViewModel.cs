using Microsoft.Xna.Framework.Graphics;
using MonogameBasicHelper.MVVM;
namespace MonogameBasicHelper.Concrete.ViewModels
{
    public interface IGraphicsViewModel : IViewModel
    {
        GraphicsDevice GraphicsDevice { get; }

    }
}
