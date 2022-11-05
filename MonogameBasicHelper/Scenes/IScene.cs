using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameBasicHelper.Scenes
{
    public interface IScene
    {
        VertexBuffer VertexBuffer { get; set; }
        IndexBuffer IndexBuffer { get; set; }
        Vector3 Position { get; set; }

        void Activate();
        void Deactivate();
        void Draw();
    }
}
