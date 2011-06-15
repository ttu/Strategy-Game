using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    // This is also system update order
    public enum SystemType : int
    {
        InputSystem = 1,
        CameraSystem = 2,
        ObjectSystem = 3
    }

    public abstract class BaseSystem
    {
        public SystemType SysType { get; protected set; }
    }
}
