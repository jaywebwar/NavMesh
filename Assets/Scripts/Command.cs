public abstract class Command
{
    RTSUnitCommandProcessor _unitCommanded;
    protected bool isInterrupted = false;
    public Command(RTSUnitCommandProcessor unit)
    {
        _unitCommanded = unit;
    }
    public abstract void Execute();
    public virtual void Interrupt() => isInterrupted = true;
    public abstract bool IsFinished { get; }
    public abstract bool OverWritesQueuedCommands { get; }
}
