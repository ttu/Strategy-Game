using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public static class ComponentManager
    {
        public static Type[] ComponentExecuteOrder = new Type[] { 
            typeof(BehaviorComponent),
            typeof(MoveComponent),
            typeof(CombatComponent)
        };
    }
}
