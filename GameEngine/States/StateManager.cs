using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine
{
    public class StateManager
    {
        BaseState _current;

        InitState _initState;
        RealTimeState _rtState;
        PauseState _pauseState;
        EndState _endState;

        int _turnCount = 0;

        public StateManager(GameArea gameArea, CameraSystem camera)
        {            
            BaseState.EndState += new ChangeState(state_EndState);
            BaseState.PauseState += new ChangeState(state_PauseState);

            _initState = new InitState(camera, gameArea);
            _rtState = new RealTimeState(camera, gameArea);
            _pauseState = new PauseState(camera, gameArea);
            _endState = new EndState(camera, gameArea);
       
            _current = _initState;
        }

        void state_PauseState()
        {
            if (_current == _pauseState)
                _current = _rtState;
            else
                _current = _pauseState;
        }

        void state_EndState()
        {
            if (_current == _initState)
                _current = _rtState;
            else if (_current == _pauseState)
                _current = _rtState;
            else if (_current == _rtState)
                _current = _endState;
            else if (_current == _endState)
                _current = _initState;
        }

        public void HandleInput(InputAction input)
        {
            _current.HandleInput(input);
        }

        public void Update(long elapsedTime)
        {
            _turnCount++;
            _current.Update(elapsedTime);
        }

        public List<DrawObject> GetDrawableObjects()
        {
            return _current.GetDrawableObjects();
        }

        public bool GameRunning()
        {
            // In here comes exit application command
            return true;
        }
    }
}
