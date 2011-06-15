using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Drawing;

namespace GameEngine
{
    public class Game
    {    
        IView _view;

        StateManager _stateManager;

        Stopwatch _upTime;
        long _lastTime;

        public Game(IView view)
        {
            _view = view;

            GameArea gameArea = new GameArea((int)view.GameViewSize.Width, (int)view.GameViewSize.Height, 6, 24);
            CameraSystem camera = new CameraSystem(view.GameViewSize, gameArea.GameAreaSize);
            _stateManager = new StateManager(gameArea, camera);

            _upTime = new Stopwatch();
        }

        public void Start()
        {
            _upTime.Start();

            while (_stateManager.GameRunning())
            {
                long gameTime = _upTime.ElapsedMilliseconds;
                long elapsedTime = gameTime - _lastTime;
                
                // Update max 60fps (1000 / 60)
                if (elapsedTime < 17)
                {
                    System.Threading.Thread.Sleep(5);  // Sleep 0,05sec
                    continue;
                }

                _lastTime = gameTime;

                _stateManager.Update(elapsedTime);       

                // Give new frame to view
                // Check that is not drawing
                // TODO: Should store frames or wait?
                while (_view.Drawing)
                    System.Threading.Thread.Sleep(5);

                _view.Draw(_stateManager.GetDrawableObjects());
            }
        }

        public void ClickAction(int x, int y)
        {
            _stateManager.HandleInput(new Click(x, y));
        }

        public void MoveCameraLocationAction(int x, int y)
        {
            _stateManager.HandleInput(new MoveCameraTagetLocation(x,y));
        }

        public void EndTurnAction()
        {
            _stateManager.HandleInput(new EndTurn());
        }

        public void SelectNextAction()
        {
            _stateManager.HandleInput(new SelectPrevNextObject(+1));
        }
    
        public void SelectPreviousAction()
        {
            _stateManager.HandleInput(new SelectPrevNextObject(-1));
        }

        public void ZoomOutAction()
        {
            _stateManager.HandleInput(new Zoom(1));
        }

        public void ZoomInAction()
        {
            _stateManager.HandleInput(new Zoom(-1));
        }

        public void MouseUpAction(int x, int y)
        {
            _stateManager.HandleInput(new MouseUpAction(x, y));
        }

        public void MouseDownAction(int x, int y)
        {
            _stateManager.HandleInput(new MouseDownAction(x, y));
        }

        public void MouseMoveAction(int x, int y)
        {
            _stateManager.HandleInput(new MouseMoveAction(x, y));
        }

        public void PauseAction()
        {
            _stateManager.HandleInput(new Pause());
        }
    }
}
