using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameBasicHelper.Concrete.Adapters;
using MonogameBasicHelper.Events;
using MonogameBasicHelper.MVVM;
using System.Collections.Generic;
using System.Threading;

namespace MonogameBasicHelper.Scenes
{
    public abstract class BaseSceneViewModel : IViewModel,
        INotificationReceiver<(IScene scene, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)>,
        INotificationReceiver<(IScene scene, IndexBuffer indexBuffer)>,
        INotificationReceiver<(IScene scene, VertexBuffer vertexBuffer)>
    {
        private List<IScene> _inactiveScenes = new List<IScene>();
        private List<IScene> _activeScenes = new List<IScene>();

        private Semaphore _semaphore = new Semaphore(1, 1);

        public GraphicsDevice GraphicsDevice { get; private set; }
        public GraphViewerCameraAdapter CameraAdapter { get; private set; }

        public BaseSceneViewModel(GraphViewerCameraAdapter camera)
        {
            CameraAdapter = camera;
        }

        public void InitializeContainer(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
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

        public void Notify(object sender, (IScene scene, VertexBuffer vertexBuffer, IndexBuffer indexBuffer) argument)
        {
            _semaphore.WaitOne();

            argument.scene.VertexBuffer?.Dispose();
            argument.scene.IndexBuffer?.Dispose();

            if (argument.vertexBuffer != null)
                argument.scene.VertexBuffer = argument.vertexBuffer;
            if (argument.indexBuffer != null)
                argument.scene.IndexBuffer = argument.indexBuffer;

            _semaphore.Release();
        }

        public void Notify(object sender, (IScene scene, VertexBuffer vertexBuffer) argument)
        {
            _semaphore.WaitOne();
            argument.scene.VertexBuffer?.Dispose();
            if (argument.vertexBuffer != null)
                argument.scene.VertexBuffer = argument.vertexBuffer;
            _semaphore.Release();
        }

        public void Notify(object sender, (IScene scene, IndexBuffer indexBuffer) argument)
        {
            _semaphore.WaitOne();
            argument.scene.IndexBuffer?.Dispose();
            if (argument.indexBuffer != null)
                argument.scene.IndexBuffer = argument.indexBuffer;
            _semaphore.Release();
        }

        public abstract void Update(GameTime gameTime);

        public virtual void DrawScene()
        {
            _semaphore.WaitOne();
            foreach (var scene in _activeScenes)
            {
                scene.Draw();
            }
            _semaphore.Release();
        }

      
    }
}