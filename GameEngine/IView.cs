using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public interface IView
    {
        void Draw(List<DrawObject> objectsToDraw);
        bool Drawing { get; }
        Size GameViewSize { get; }
    }
}
