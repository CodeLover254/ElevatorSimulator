namespace ElevatorSimulator.Domain.Models;

public class FreightElevator: BaseElevator
{
    public int MaximumWeight { get; set; }
    public int CurrentWeight { get; set; }

    public override bool IsFullyLoaded()
    {
        return MaximumWeight == CurrentWeight;
    }

    public override int RemainingCapacity()
    {
        return MaximumWeight - CurrentWeight;
    }

    public override string GetStatus()
    {
        throw new NotImplementedException();
    }
}