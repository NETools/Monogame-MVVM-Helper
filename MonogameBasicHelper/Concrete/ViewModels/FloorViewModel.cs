using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameBasicHelper.Attributes;
using MonogameBasicHelperDLL.Concrete.Adapters;
using MonogameBasicHelperDLL.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Concrete.ViewModels
{
    public class FloorViewModel : IGraphicsViewModel, IScene
    {

        private BasicEffect _basicEffect;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public int Width { get; private set; }
        public int Depth { get; private set; }

        public GraphViewerCameraAdapter CameraAdapter { get; private set; }
        public FloorAdapter FloorAdapter { get; private set; }

        public Vector3 Position { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }

        public FloorViewModel(GraphViewerCameraAdapter cameraAdapter, FloorAdapter floorAdapter)
        {
            CameraAdapter = cameraAdapter;
            FloorAdapter = floorAdapter;
        }

        [OnBuiltUp]
        public void OnInitialize(GraphicsDevice graphicsDevice, int w, int d)
        {
            GraphicsDevice = graphicsDevice;

            Width = w;
            Depth = d;

            Position = new Vector3(Width * .5f, 0, Depth * .5f);

            _basicEffect = new BasicEffect(GraphicsDevice);
        }

        public void Activate()
        {
            FloorAdapter.Load();

        }

        public void Deactivate()
        {

        }

        public void Draw()
        {
            _basicEffect.View = CameraAdapter.ViewMatrix;
            _basicEffect.Projection = CameraAdapter.ProjectionMatrix;
            _basicEffect.World = Matrix.CreateTranslation(-CameraAdapter.RotatedCameraPosition - Position);
            _basicEffect.VertexColorEnabled = true;
            _basicEffect.CurrentTechnique.Passes[0].Apply();

            GraphicsDevice.SetVertexBuffer(VertexBuffer);
            GraphicsDevice.Indices = IndexBuffer;

            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, IndexBuffer.IndexCount / 2);
        }
    }
}
