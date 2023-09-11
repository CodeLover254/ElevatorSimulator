using ElevatorSimulator.Domain.Comparers;
using ElevatorSimulator.Domain.Enums;

namespace ElevatorSimulator.Domain.Models;

public abstract class BaseElevator
{
    public string Label { get; set; }
    public int CurrentFloor { get; set; } = 0;
    public int LowestFloor { get; set; }
    public int HighestFloor { get; set; }
    public ElevatorType ElevatorType { get; set; }
    protected Direction Direction = Direction.Neutral;
    protected ElevatorState ElevatorState = ElevatorState.Idle;
    protected readonly PriorityQueue<Request, Request> UpwardQueue = new PriorityQueue<Request, Request>(new RequestInverseComparer());
    protected readonly PriorityQueue<Request, Request> DownwardQueue = new PriorityQueue<Request, Request>(new RequestComparer());

    public abstract bool IsFullyLoaded();
    public abstract int RemainingCapacity();
    public abstract int GetMaximumCapacity();
    public abstract string GetStatus();
    public abstract void ModifyLoading(int number, ElevatorLoadingOptions loadingOptions);

    public virtual int GetAvailabilityScore(Request request)
    {
        //Check for matching request type and if the elevator is fully loaded
        if (request.RequestType != ElevatorType || IsFullyLoaded()) return (int)ElevatorAvailability.Unavailable;
        var score = 0;
        //closeness to the requester as a scoring factor
        var distance = Math.Abs(CurrentFloor - request.SourceFloor);
        score += distance == 0 ? HighestFloor : (HighestFloor-1) / distance;
        //direction as a scoring factor
        if (request.Direction == Direction)
        {
            score += HighestFloor;
        }else if (Direction == Direction.Neutral)
        {
            score += HighestFloor/2;
        }
        //elevator loading as a factor
        score += (RemainingCapacity() * HighestFloor) / GetMaximumCapacity();

        return score;
    }

    public bool ReceiveRequest(Request request)
    {
        if (request.Direction == Direction.Up)
        {
            if (CurrentFloor > request.SourceFloor) return false;//the elevator could have passed already
            UpwardQueue.Enqueue(request,request);
        }
        else
        {
            if (CurrentFloor < request.SourceFloor) return false;
            DownwardQueue.Enqueue(request, request);
        }

        return true;
    }

    private void Run()
    {
        while (true)
        {
            if (ElevatorState == ElevatorState.Idle)
            {
                if (CurrentFloor < HighestFloor && UpwardQueue.Count > 0 && (Direction == Direction.Up || Direction == Direction.Neutral))
                {
                    Move(Direction.Up);
                }else if (CurrentFloor > LowestFloor && DownwardQueue.Count > 0 && (Direction == Direction.Down || Direction == Direction.Neutral))
                {
                    Move(Direction.Down);
                }
            }else if (ElevatorState == ElevatorState.Moving)
            {
                var recentRequest = Direction == Direction.Up ? UpwardQueue.Peek() : DownwardQueue.Peek();
                DockIfNeeded(recentRequest);
            }
            else
            {
                HandleDockedState();
            }
            
            Thread.Sleep(1000);
        }
    }

    private void Move(Direction direction)
    {
        ElevatorState = ElevatorState.Moving;
        Direction = direction;
        switch (direction)
        {
            case Direction.Up:
                CurrentFloor++;
                break;
            case Direction.Down:
                CurrentFloor--;
                break;
        }
    }

    private void DockIfNeeded(Request recentRequest)
    {
        if (CurrentFloor == recentRequest.DestinationFloor || CurrentFloor == recentRequest.SourceFloor)
        {
            ElevatorState = ElevatorState.Docked;
        }
        else
        {
            if (!ReachedThresholdFloor())
            {
                Move(Direction);
            }
        }
    }

    private void HandleDockedState()
    {
        var targetQueue = Direction == Direction.Up ? UpwardQueue : DownwardQueue;
        var otherQueue = targetQueue == UpwardQueue ? DownwardQueue : UpwardQueue;
        var otherDirection = Direction == Direction.Up ? Direction.Down : Direction.Up;
        //load or unload
        var elevatorRequest = targetQueue.Dequeue();
        if (CurrentFloor == elevatorRequest.DestinationFloor)
        {
            ModifyLoading(elevatorRequest.Capacity, ElevatorLoadingOptions.Remove);
        }

        if (CurrentFloor == elevatorRequest.SourceFloor)
        {
            ModifyLoading(elevatorRequest.Capacity, ElevatorLoadingOptions.Add);
        }
        //check if queue has more items then move in that direction. 
        if (targetQueue.Count > 0)
        {
            Move(Direction);
            return;
        }
        //otherwise check if other queue has items and move in the other direction
        if (otherQueue.Count > 0)
        {
            Move(otherDirection);
            return;
        }
        //else just stay idle and wait for assignment
        Direction = Direction.Neutral;
        ElevatorState = ElevatorState.Idle;
    }

    private bool ReachedThresholdFloor()
    {
        return CurrentFloor == HighestFloor || CurrentFloor == LowestFloor;
    }

    public void Activate()
    {
        Thread elevatorThread = new Thread(Run);
        elevatorThread.Start();
    }
    
}