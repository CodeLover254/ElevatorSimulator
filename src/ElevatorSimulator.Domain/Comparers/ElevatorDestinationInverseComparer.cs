using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Domain.Comparers;

public class ElevatorDestinationInverseComparer: IComparer<ElevatorDestination>
{
    public int Compare(ElevatorDestination? x, ElevatorDestination? y)
    {
        if (x.FloorNumber == y.FloorNumber) return 0;
        if (x.FloorNumber < y.FloorNumber) return 1;
        return -1;
    }
}