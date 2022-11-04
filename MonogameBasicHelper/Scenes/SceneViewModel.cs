using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameBasicHelperDLL.Concrete.Adapters;
using MonogameBasicHelperDLL.Events;
using MonogameBasicHelperDLL.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Scenes
{
    public abstract class BaseSceneViewModel : IViewModel,
        INotificationReceiver<(IScene, VertexBuffer, IndexBuffer)>
    {
        private List<IScene> _inactiveScenes = new List<IScene>();
        private List<IScene> _activeScenes = new List<IScene>();

        public GraphicsDevice GraphicsDevice { get; private set; }
        public GraphViewerCameraAdapter CameraAdapter { get; private set; }

        public BaseSceneViewModel(GraphViewerCameraAdapter camera)
        {
            this.CameraAdapter = camera;
        }

        public void InitializeContainer(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
        }

        public void AddScene(IScene viewModel)
        {
            _inactiveScenes.Add(viewModel);
        }

        public void ActivateScene(IScene viewModel)
        {
            if (_inactiveScenes.Contains(viewModel))
            {
                _inactiveScenes.Remove(viewModel);
                if (!_activeScenes.Contains(viewModel))
                {
                    _activeScenes.Add(viewModel);
                    viewModel.Activate();
                }
            }
        }

        public void ChangeActiveScene(IScene fromActiveScene, IScene toInactiveScene)
        {
            _inactiveScenes.Add(fromActiveScene);
            _activeScenes.Remove(fromActiveScene);

            _activeScenes.Add(toInactiveScene);
            _inactiveScenes.Remove(toInactiveScene);


        }

        public void Notify(object sender, (IScene, VertexBuffer, IndexBuffer) argument)
        {
            argument.Item1.VertexBuffer?.Dispose();
            argument.Item1.IndexBuffer?.Dispose();

            if (argument.Item3 != null)
                argument.Item1.VertexBuffer = argument.Item2;
            if (argument.Item3 != null)
                argument.Item1.IndexBuffer = argument.Item3;
        }

        public abstract void Update(GameTime gameTime);

        public virtual void DrawScene()
        {
            foreach (var scene in _activeScenes)
            {
                scene.Draw();
            }
        }
    }
}