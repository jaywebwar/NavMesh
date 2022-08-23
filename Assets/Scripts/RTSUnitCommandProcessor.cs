using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(RTSUnit))]
public class RTSUnitCommandProcessor : MonoBehaviour
{
    Queue<Command> _commands;
    NavMeshAgent _nma;
    Command _currentCommand;


    private void Awake()
    {
        _commands = new Queue<Command>();
        _nma = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        ProcessCommands();
    }

    public void QueueCommand(Command cmd)
    {
        if (cmd.OverWritesQueuedCommands)
        {
            _commands.Clear();

            if(_currentCommand != null)
            {
                _currentCommand.Interrupt();
            }
            
        }
            
        _commands.Enqueue(cmd);
    }

    void ProcessCommands()
    {
        if(_currentCommand == null)
        {
            if(_commands.Count != 0)
            {
                _currentCommand = _commands.Dequeue();
                _currentCommand.Execute();
            }
        }
        else
        {
            if(_currentCommand.IsFinished)
            {
                if (_commands.Count != 0)
                {
                    _currentCommand = _commands.Dequeue();
                    _currentCommand.Execute();
                }
                else
                {
                    _currentCommand = null;
                }
            }
        }
    }

}

