using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace GameEngine
{
    public delegate void DoAction(InputAction action);

    public class InputSystem : BaseSystem
    {
        public event DoAction DoAction;

        // Inputs for handling
        Queue<InputAction> _currentInputs = new Queue<InputAction>();
        // Inputs waiting for handling
        Queue _inputQueue = new Queue();

        public InputSystem()
        {
            SysType = SystemType.InputSystem;
        }

        public void HandleInputs()
        {
            // Nobody cares about actions.. Do nothing
            if (DoAction == null)
                return;

            // Move from waiting queue to performing queue
            // Not sure if this saves from any ui lag cos its so fast
            lock (_inputQueue.SyncRoot)
            {
                while (_inputQueue.Count > 0)
                {
                    _currentInputs.Enqueue(_inputQueue.Dequeue() as InputAction);
                }
            }

            while (_currentInputs.Count > 0)
            {
                InputAction current = _currentInputs.Dequeue();
                DoAction(current);
            }
        }

        public void AddInput(InputAction input)
        {
            _inputQueue.Enqueue(input);
        }
    }
}
