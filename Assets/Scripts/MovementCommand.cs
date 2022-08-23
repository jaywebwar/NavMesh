using UnityEngine;
using UnityEngine.AI;

public class MovementCommand : Command
{
    NavMeshAgent _navMeshAgent;
    Vector3 _destination;
    bool _overWritesPreviousCommands;
    RTSUnit _unit;

    //this notation passes the parameter unit to the base class constructor to run it as well
    public MovementCommand(RTSUnitCommandProcessor unit, Vector3 destination, NavMeshAgent agent, bool overWrite) : base(unit)
    {
        _destination = destination;
        _navMeshAgent = agent;
        _overWritesPreviousCommands = overWrite;
        _unit = unit.GetComponent<RTSUnit>();

        unit.QueueCommand(this);
    }

    public override void Execute()
    {
        _unit.HandleMovement(_destination);
    }

    public override bool IsFinished => _navMeshAgent.isStopped || isInterrupted;

    public override bool OverWritesQueuedCommands => _overWritesPreviousCommands;
}
