using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    // Load all objects etc.
    public class InitState : BaseState
    {
        public InitState(CameraSystem camera, GameArea gameArea)
            : base(camera, gameArea)
        {
            _allowedInputs = new Type[] { typeof(EndTurn) };
        }

        public override List<DrawObject> GetDrawableObjects()
        {
            List<DrawObject> drawList = new List<DrawObject>();

            System.Drawing.Image loading = new System.Drawing.Bitmap(System.IO.Path.Combine("Images", "Loading.png"));

            DrawObject drawO = new DrawObject(loading, _cameraSystem.OwnCenter.X, _cameraSystem.OwnCenter.Y, 1);
            drawList.Add(drawO);

            return drawList;
        }

        bool first = true;

        public override void Update(long elapsedTime)
        {
            // Skip first so will show loading image
            if (first)
            {
                first = false;
                return;
            }

            System.Threading.Thread.Sleep(500);

            _objectSystem.Initialize(_gameArea.GameAreaSize);

            CallEndState();
        }
    }

    // Game paused, player can move camera
    public class PauseState : BaseState
    {
        public PauseState(CameraSystem camera, GameArea gameArea)
            : base(camera, gameArea)
        {
            _allowedInputs = new Type[] { typeof(Zoom), typeof(MoveCameraTagetLocation), 
                typeof(SelectPrevNextObject), typeof(Click) , typeof(SelectPrevNextObject), typeof(EndTurn), typeof(Pause),
                typeof(MouseDownAction),typeof(MouseUpAction), typeof(MouseMoveAction)};
        }

        public override List<DrawObject> GetDrawableObjects()
        {
            List<DrawObject> drawList = base.GetDrawableObjects();

            System.Drawing.Image loading = new System.Drawing.Bitmap(System.IO.Path.Combine("Images", "Pause.png"));

            DrawObject drawO = new DrawObject(loading, _cameraSystem.OwnCenter.X, _cameraSystem.OwnCenter.Y, 1);
            drawList.Add(drawO);

            return drawList;
        }

        public override void Update(long elapsedTime)
        {
            _inputSystem.HandleInputs();

            _cameraSystem.MoveLocation(elapsedTime, 500);
        }
    }

    // Game ended
    public class EndState : BaseState
    {
        public EndState(CameraSystem camera, GameArea gameArea):base(camera, gameArea)
        {
            _allowedInputs = new Type[] { typeof(Click), typeof(EndTurn)};
        }

        public override void Update(long elapsedTime)
        {
            _inputSystem.HandleInputs();
        }

        public override List<DrawObject> GetDrawableObjects()
        {
            List<DrawObject> drawList = base.GetDrawableObjects();

            System.Drawing.Image loading = new System.Drawing.Bitmap(System.IO.Path.Combine("Images", "End.png"));

            DrawObject drawO = new DrawObject(loading, _cameraSystem.OwnCenter.X, _cameraSystem.OwnCenter.Y, 1);
            drawList.Add(drawO);

            return drawList;
        }
    }

    public class RealTimeState : BaseState
    {
        public RealTimeState(CameraSystem camera, GameArea gameArea)
            : base(camera, gameArea)
        {
            _allowedInputs = new Type[] { typeof(Zoom), typeof(MoveCameraTagetLocation), 
                typeof(SelectPrevNextObject), typeof(Click) , typeof(SelectPrevNextObject), 
                typeof(MouseDownAction),typeof(MouseUpAction), typeof(MouseMoveAction), typeof(EndTurn), typeof(Pause)};
        }
    }

    // These two states are for turn based game
    public class PlayerState : BaseState
    {
        public PlayerState(CameraSystem camera, GameArea gameArea)
            : base(camera, gameArea)
        {
            _allowedInputs = new Type[] { typeof(Zoom), typeof(MoveCameraTagetLocation), 
            typeof(SelectPrevNextObject), typeof(Click) , typeof(SelectPrevNextObject), 
            typeof(MouseDownAction),typeof(MouseUpAction), typeof(MouseMoveAction), typeof(EndTurn)};
        }
    }

    public class AIState : BaseState
    {
        public AIState(CameraSystem camera, GameArea gameArea)
            : base(camera, gameArea)
        {
            _allowedInputs = new Type[] { typeof(Zoom), typeof(MoveCameraTagetLocation), 
            typeof(SelectPrevNextObject), typeof(Click) , typeof(SelectPrevNextObject), 
            typeof(MouseDownAction),typeof(MouseUpAction), typeof(MouseMoveAction), typeof(EndTurn)};
        }
    }
}
