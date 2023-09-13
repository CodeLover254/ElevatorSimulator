using ElevatorSimulator.Domain.Enums;
using ElevatorSimulator.Domain.Exceptions;

namespace ElevatorSimulator.Domain.Models;

public class FreightElevator: BaseElevator
{
    public int MaximumWeight { get; set; }
    public int CurrentWeight { get; set; } = 0;
    protected override ElevatorType ElevatorType { get; set; } = ElevatorType.Freight;

    public override bool IsFullyLoaded()
    {
        return MaximumWeight == CurrentWeight;
    }

    public override int RemainingCapacity()
    {
        return MaximumWeight - CurrentWeight;
    }

    public override int GetMaximumCapacity()
    {
        return MaximumWeight;
    }

    public override string GetStatus()
    {
        return $"Elevator {Label} is {ElevatorState} at Floor {CurrentFloor}. Freight onboard is {CurrentWeight} Kgs";
    }

    public override void ModifyLoading(int number, ElevatorLoadingOptions loadingOptions)
    {
        if (loadingOptions == ElevatorLoadingOptions.Add)
        {
            if (CurrentWeight + number > MaximumWeight) throw new DomainException("Maximum weight cannot be exceeded");
            CurrentWeight += number;
            return;
        }

        CurrentWeight -= number;
    }
}