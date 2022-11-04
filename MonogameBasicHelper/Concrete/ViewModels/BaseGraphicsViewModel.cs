using Microsoft.Xna.Framework.Graphics;
using MonogameBasicHelperDLL.MVVM;


namespace MonogameBasicHelperDLL.Concrete.ViewModels
{
    public interface IGraphicsViewModel : IViewModel
    {
        GraphicsDevice GraphicsDevice { get; }

    }
}
