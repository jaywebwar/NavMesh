using UnityEngine.AI;

public class AttackCommand : Command
{
    NavMeshAgent _nma;
    RTSUnit _enemy;
    bool _overwritesPreviousCommands;
    RTSUnit _unit;
    bool targetIsDead;
    bool targetIsOutsideOfVision;

    public AttackCommand(RTSUnitCommandProcessor unit, NavMeshAgent nma, RTSUnit target, bool overWrite) : base(unit)
    {
        _nma = nma;
        _enemy = target;
        _overwritesPreviousCommands = overWrite;
        _unit = unit.GetComponent<RTSUnit>();

        unit.QueueCommand(this);
    }
    public override bool IsFinished => isInterrupted || targetIsDead || targetIsOutsideOfVision;

    public override bool OverWritesQueuedCommands => _overwritesPreviousCommands;

    public override void Execute()
    {
        _unit.IsInAttackingState = true;
        _unit.EnterAttackingState(_enemy);
    }

    public override void Interrupt()
    {
        base.Interrupt();
        _unit.IsInAttackingState = false;
    }
}