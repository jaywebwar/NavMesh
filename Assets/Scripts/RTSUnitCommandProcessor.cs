using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTSUnit))]
[RequireComponent(typeof(PlayerInputController))]
public class RTSUnitCommandProcessor : MonoBehaviour
{
    Queue<Command> _commands;
    Command _currentCommand;

    PlayerInputController _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        ListenForCommands();
        ProcessCommands();
    }

    void ListenForCommands()
    {
        ListenForNPCCommands();
        if(GetComponent<IRTSUnit>().IsPlayersUnit)
        {
            ListenForPlayerCommands();
        }
    }

    private void ListenForPlayerCommands()
    {

    }

    private void ListenForNPCCommands()
    {
        //Listen for awareness collider to report an enemy triggering the collider
    }

    void ProcessCommands()
    {
        if(_currentCommand != null)
        {

        }

    }

}

