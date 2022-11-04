using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Scenes
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
