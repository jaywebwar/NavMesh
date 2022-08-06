public abstract class Command
{
    RTSUnitCommandProcessor _unitCommanded;
    public Command(RTSUnitCommandProcessor unit)
    {
        _unitCommanded = unit;
    }
    public abstract void Execute();
    public abstract bool IsFinished { get; }
    public abstract bool OverWritesQueuedCommands { get; }
}
