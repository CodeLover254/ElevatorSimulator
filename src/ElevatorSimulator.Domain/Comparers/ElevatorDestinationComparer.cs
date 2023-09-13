using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Domain.Comparers;

public class ElevatorDestinationComparer: IComparer<ElevatorDestination>
{
    public int Compare(ElevatorDestination? x, ElevatorDestination? y)
    {
        return x.FloorNumber.CompareTo(y.FloorNumber);
    }
}