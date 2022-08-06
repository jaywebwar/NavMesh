using UnityEngine;
using UnityEngine.AI;

public class MovementCommand : Command
{
    NavMeshAgent _navMeshAgent;
    Vector3 _destination;

    //this notation passes the parameter unit to the base class constructor to run it as well
    public MovementCommand(RTSUnitCommandProcessor unit, Vector3 destination, NavMeshAgent agent) : base(unit)
    {
        _destination = destination;
        _navMeshAgent = agent;
    }

    public override void Execute()
    {
        _navMeshAgent.SetDestination(_destination);
    }

    public override bool IsFinished => _navMeshAgent.remainingDistance <= 0.1f;

    public override bool OverWritesQueuedCommands => true;
}