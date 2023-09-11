using ElevatorSimulator.Domain.Models;

namespace ElevatorSimulator.Domain.Comparers;

public class RequestInverseComparer: IComparer<Request>
{
    public int Compare(Request? x, Request? y)
    {
        if (x.SourceFloor == y.SourceFloor) return 0;
        if (x.SourceFloor < y.SourceFloor) return 1;
        return -1;
    }
}
