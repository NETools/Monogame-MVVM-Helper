using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonogameBasicHelperDLL.Concrete.ViewModels;
using MonogameBasicHelperDLL.ContainerService;
using MonogameBasicHelperDLL.Events;
using MonogameBasicHelperDLL.MVVM;
using MonogameBasicHelperDLL.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Concrete.Adapters
{
    public class FloorAdapter : IAdapter
    {
        private FloorViewModel FloorViewModel => MonogameDepdencyInjection.Services.GetViewModel<FloorViewModel>();
        private INotificationAdapter NotificationAdapter { get; }
        public FloorAdapter(INotificationAdapter notificationAdapter)
        {
            this.NotificationAdapter = notificationAdapter;
        }

        public void Load()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[2 * (FloorViewModel.Width + FloorViewModel.Depth + 2)];

            int index = 0;
            for (int z = 0; z <= FloorViewModel.Depth; z++)
            {
                vertices[index++] = new VertexPositionColor(new Vector3(0, 0, z), Color.Gray);
                vertices[index++] = new VertexPositionColor(new Vector3(FloorViewModel.Width, 0, z), Color.Gray);
            }

            for (int x = 0; x <= FloorViewModel.Width; x++)
            {
                vertices[index++] = new VertexPositionColor(new Vector3(x, 0, 0), Color.Gray);
                vertices[index++] = new VertexPositionColor(new Vector3(x, 0, FloorViewModel.Depth), Color.Gray);
            }

            int[] indices = new int[2 * (FloorViewModel.Width + FloorViewModel.Depth + 2)];

            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = i;
            }

            var vertexBuffer = new VertexBuffer(FloorViewModel.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            var indexBuffer = new IndexBuffer(FloorViewModel.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indices);

            NotificationAdapter.Notify<INotificationReceiver<(IScene, VertexBuffer, IndexBuffer)>>(this, ((IScene)FloorViewModel, vertexBuffer, indexBuffer));
        }

    }
}
