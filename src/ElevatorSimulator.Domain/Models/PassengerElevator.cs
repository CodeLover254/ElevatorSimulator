using ElevatorSimulator.Domain.Enums;
using ElevatorSimulator.Domain.Exceptions;

namespace ElevatorSimulator.Domain.Models;

public class PassengerElevator: BaseElevator
{
    public int MaximumPassengers { get; set; }
    public int Passengers { get; set; } = 0;

    public override bool IsFullyLoaded()
    {
        return Passengers == MaximumPassengers;
    }

    public override int RemainingCapacity()
    {
        return MaximumPassengers - Passengers;
    }

    public override int GetMaximumCapacity()
    {
        return MaximumPassengers;
    }

    public override string GetStatus()
    {
        return $"Elevator {Label} is {ElevatorState} at Floor {CurrentFloor} carrying {Passengers} passengers";
    }

    public override void ModifyLoading(int number, ElevatorLoadingOptions loadingOptions)
    {
        if (loadingOptions == ElevatorLoadingOptions.Add)
        {
            if (Passengers + number > MaximumPassengers) throw new DomainException("Maximum number of passengers cannot be exceeded");
            Passengers += number;
            return;
        }

        Passengers -= number;
    }
}