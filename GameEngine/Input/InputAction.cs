using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public enum ActionType
    {
        EndTurn,
        Pause,
        Click,
        Zoom,
        MoveCameraTargetLocation,
        MouseUp,
        MouseDown,
        MouseMove,
        SelectPrevNextObject
    }

    /*
     * Base class for all actions 
     */
    public abstract class InputAction
    {
        public ActionType InputType;

        protected int _x;
        protected int _y;

        public int X { get { return _x; } }
        public int Y { get { return _y; } }
    }

    public class Click : InputAction
    {
        public Click(int x, int y)
        {
            InputType = ActionType.Click;
            _x = x;
            _y = y;
        }
    }

    public class MoveCameraTagetLocation : InputAction
    {
        public MoveCameraTagetLocation(int x, int y)
        {
            InputType = ActionType.MoveCameraTargetLocation;
            _x = x;
            _y = y;
        }
    }

    public class Zoom : InputAction
    {
        int _zoomModifier;

        public int ZoomModifier { get { return _zoomModifier; } }

        public Zoom(int zoomModifier)
        {
            InputType = ActionType.Zoom;
            _zoomModifier = zoomModifier;
        }
    }

    public class SelectPrevNextObject : InputAction
    {
        int _direction;

        public int Direction { get { return _direction; } }

        public SelectPrevNextObject(int direction)
        {
            InputType = ActionType.SelectPrevNextObject;
            _direction = direction;
        }
    }

    public class MouseUpAction : InputAction
    {
        public MouseUpAction(int x, int y)
        {
            InputType = ActionType.MouseUp;
            _x = x;
            _y = y;
        }
    }

    public class MouseDownAction : InputAction
    {
        public MouseDownAction(int x, int y)
        {
            InputType = ActionType.MouseDown;
            _x = x;
            _y = y;
        }
    }

    public class MouseMoveAction : InputAction
    {
        public MouseMoveAction(int x, int y)
        {
            InputType = ActionType.MouseMove;
            _x = x;
            _y = y;
        } 
    }

    public class Pause : InputAction
    {
        public Pause()
        {
            InputType = ActionType.Pause;
        }
    }

    public class EndTurn : InputAction
    {
        public EndTurn()
        {
            InputType = ActionType.EndTurn;
        }
    }
}
