using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public delegate void ChangeState();

    public abstract class BaseState
    {
        public static event ChangeState EndState;
        public static event ChangeState PauseState;
       
        protected CameraSystem _cameraSystem;
        protected GameArea _gameArea;

        protected ObjectSystem _objectSystem;
        protected InputSystem _inputSystem;

        // By default do not allow anything
        protected Type[] _allowedInputs = new Type[] { };

        public BaseState(CameraSystem camera, GameArea gameArea)
        {
            _inputSystem = new InputSystem();
            _inputSystem.DoAction += new DoAction(InputManager_DoAction);

            _objectSystem = ObjectSystem.GetInstance();
            _cameraSystem = camera;
            _gameArea = gameArea;
        }

        void InputManager_DoAction(InputAction action)
        {
            switch (action.InputType)
            {
                case ActionType.EndTurn:
                    CallEndState();
                    break;
                case ActionType.Pause:
                    CallPauseState();
                    break;
                case ActionType.Click:
                    ClickAction(action as Click);
                    break;
                case ActionType.Zoom:
                    ZoomAction(action as Zoom);
                    break;
                case ActionType.MoveCameraTargetLocation:
                    MoveCameraTargetLocationAction(action as MoveCameraTagetLocation);
                    break;
                case ActionType.MouseUp:
                    MouseUpAction(action as MouseUpAction);
                    break;
                case ActionType.MouseDown:
                    MouseDownAction(action as MouseDownAction);
                    break;
                case ActionType.MouseMove:
                    MouseMoveAction(action as MouseMoveAction);
                    break;
                case ActionType.SelectPrevNextObject:
                    SelectPrevNextObjectAction(action as SelectPrevNextObject);
                    break;
                default:
                    break;
            }
        }

        public void ZoomAction(Zoom action)
        {
            _cameraSystem.Zoom += action.ZoomModifier;
        }

        public void ClickAction(Click action)
        {
            Point coordinatesInArea = _cameraSystem.GetAreaCoordinates(action.X, action.Y);

            // Area should be 2 x 2
            Rectangle rec = new Rectangle(coordinatesInArea.X - 1, coordinatesInArea.Y - 1, 2, 2);
            List<GameObject> selected = _objectSystem.Query(rec);

            // Only move the camera
            if (selected.Count == 0)
                _cameraSystem.SetTargetLocation((int)coordinatesInArea.X, (int)coordinatesInArea.Y);
            // TODO: Now choose first
            else if (selected.Count > 0)
                _cameraSystem.Target = selected[0];
        }

        public void MoveCameraTargetLocationAction(MoveCameraTagetLocation action)
        {
            _cameraSystem.MoveTargetLocation(action.X, action.Y);
        }

        public void SelectPrevNextObjectAction(SelectPrevNextObject action)
        {
            GameObject go = _objectSystem.SelectPrevNextObject(action.Direction);
            _cameraSystem.Target = go;
        }

        public void MouseUpAction(MouseUpAction action)
        {
            if (_objectSystem.DraggingObject == null)
            {
                return;
            }

            Point coordinatesInArea = _cameraSystem.GetAreaCoordinates(action.X, action.Y);

            // If Releas point is very close to original point do nothing
            if (MathHelper.Length(_objectSystem.DraggingObject.Location, coordinatesInArea) > _objectSystem.DraggingObject.Radius)
            {
                Character move = _objectSystem.DraggingObject as Character;

                // TODO: Check if enemies nearby
                // Area should be 5 x 5
                Rectangle rec = new Rectangle(coordinatesInArea.X - 1, coordinatesInArea.Y - 1, 5, 5);
                List<GameObject> selected = _objectSystem.Query(rec);

                // There should only be one
                if (selected.Count > 0 && move.Enemies.Contains(selected[0]))
                {
                    move.Target = selected[0];
                    move.TargetSetByUser = true;
                }
                else
                {
                    move.Target = null;
                    move.TargetSetByUser = false;
                    move.MoveTo = coordinatesInArea;
                }
            }

            _objectSystem.DraggingObject = null;
            _objectSystem.DraggingObjectDrawObject = null;
        }

        public void MouseDownAction(MouseDownAction action)
        {
            Point coordinatesInArea = _cameraSystem.GetAreaCoordinates(action.X, action.Y);

            // Area should be 2 x 2
            Rectangle rec = new Rectangle(coordinatesInArea.X - 1, coordinatesInArea.Y - 1, 2, 2);
            List<GameObject> selected = _objectSystem.Query(rec);

            if (selected.Count > 0)
                _objectSystem.DraggingObject = selected[0];
        }

        public void MouseMoveAction(MouseMoveAction action)
        {
            if (_objectSystem.DraggingObject == null)
                return;

            DrawObject drawO = _objectSystem.DraggingObject.ConvertToDrawObject(_cameraSystem.UpLeft.X, _cameraSystem.UpLeft.Y, _cameraSystem.Zoom);
            drawO.X = action.X;
            drawO.Y = action.Y;

            _objectSystem.DraggingObjectDrawObject = drawO;
        }

        // In here check that input is valid for state etc.
        public virtual void HandleInput(InputAction input)
        {
            if (_allowedInputs.Contains(input.GetType()))
                _inputSystem.AddInput(input);
        }

        public virtual void Update(long elapsedTime)
        {
            // TODO: Should make collection of systems and update thos in order?
            _inputSystem.HandleInputs();

            _cameraSystem.MoveLocation(elapsedTime, 500);
            _objectSystem.Update(elapsedTime);

            // TODO: Should this check if needs to change state here?           
        }

        public virtual List<DrawObject> GetDrawableObjects()
        {
            List<GameObject> objects = _objectSystem.Query(_cameraSystem.ScreenRectangle);

            List<DrawObject> drawList = new List<DrawObject>();
            drawList.Add(_gameArea.ConvertToDrawObject(_cameraSystem.ScreenRectangle, _cameraSystem.OwnCenter.X, _cameraSystem.OwnCenter.Y));

            foreach (GameObject c in objects)
                drawList.Add(c.ConvertToDrawObject(_cameraSystem.UpLeft.X, _cameraSystem.UpLeft.Y, _cameraSystem.Zoom));

            if (_objectSystem.DraggingObjectDrawObject != null)
                drawList.Add(_objectSystem.DraggingObjectDrawObject);

            return drawList;
        }

        public void CallEndState()
        {
            if (EndState != null)
                EndState();
        }

        public void CallPauseState()
        {
            if (PauseState != null)
                PauseState();
        }
    }
}
