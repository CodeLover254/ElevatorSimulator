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
    protected readonly PriorityQueue<ElevatorDestination, ElevatorDestination> UpwardQueue = new(new ElevatorDestinationComparer());
    protected readonly PriorityQueue<ElevatorDestination, ElevatorDestination> DownwardQueue = new(new ElevatorDestinationInverseComparer());

    public abstract bool IsFullyLoaded();
    public abstract int RemainingCapacity();
    public abstract int GetMaximumCapacity();
    public abstract string GetStatus();
    public abstract void ModifyLoading(int number, ElevatorLoadingOptions loadingOptions);
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public int GetAvailabilityScore(Request request)
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

    private Direction DetermineDefaultDirection(Request request)
    {
        var direction = Direction.Up;
        if (request.SourceFloor < CurrentFloor)
        {
            direction = Direction.Down;
        }else if (request.SourceFloor == CurrentFloor)
        {
            direction = request.DestinationFloor > CurrentFloor ? Direction.Up : Direction.Down;
        }

        return direction;
    }
     
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public int ScheduleRequest(Request request)
    {
        var boarding = Math.Min(RemainingCapacity(), request.Capacity);
        request.Capacity = boarding;

        var direction = DetermineDefaultDirection(request);
        //if elevator is idle the first request sets direction
        if (ElevatorState == ElevatorState.Idle) Direction = direction;
        
        if (request.SourceFloor != CurrentFloor)
        {
            //in this case 2 destinations need to be set. One for the source. The other for the destination
            var firstDestination = new ElevatorDestination
            {
                Capacity = boarding,
                DestinationType = ElevatorDestinationType.PickUp,
                FloorNumber = request.SourceFloor
            };

            var finalDestination = new ElevatorDestination
            {
                Capacity = boarding,
                DestinationType = ElevatorDestinationType.DropOff,
                FloorNumber = request.DestinationFloor
            };

            var firstDestinationQueue = direction == Direction.Up ? UpwardQueue : DownwardQueue;
            firstDestinationQueue.Enqueue(firstDestination,firstDestination);

            var finalDestinationQueue = request.SourceFloor < request.DestinationFloor ? UpwardQueue : DownwardQueue;
            finalDestinationQueue.Enqueue(finalDestination, finalDestination);
        }
        else
        {
            //when source is similar to current floor set only one destination
            ModifyLoading(boarding, ElevatorLoadingOptions.Add);
            var targetQueue = direction == Direction.Up ? UpwardQueue : DownwardQueue;
            var destination = new ElevatorDestination
            {
                Capacity = boarding,
                DestinationType = ElevatorDestinationType.DropOff,
                FloorNumber = request.DestinationFloor
            };
            targetQueue.Enqueue(destination,destination);
        }
        
        return boarding;
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    private void Run()
    {
        while (true)
        {
            if (ElevatorState == ElevatorState.Idle)
            {
                if (Direction != Direction.Neutral)
                {
                    Move(Direction);
                }
            }else if (ElevatorState == ElevatorState.Moving)
            {
                var nextDestination = Direction == Direction.Up ? UpwardQueue.Peek() : DownwardQueue.Peek();
                DockIfNeeded(nextDestination);
            }
            else
            {
                HandleDockedState();
            }
            
            Thread.Sleep(2000);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
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
     
    /// <summary>
    /// 
    /// </summary>
    /// <param name="destination"></param>
    private void DockIfNeeded(ElevatorDestination destination)
    {
        if (CurrentFloor == destination.FloorNumber)
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
     
    /// <summary>
    /// 
    /// </summary>
    private void HandleDockedState()
    {
        var targetQueue = Direction == Direction.Up ? UpwardQueue : DownwardQueue;
        var otherQueue = targetQueue == UpwardQueue ? DownwardQueue : UpwardQueue;
        var otherDirection = Direction == Direction.Up ? Direction.Down : Direction.Up;
       
        var elevatorDestination = targetQueue.Dequeue();
        if (elevatorDestination.DestinationType == ElevatorDestinationType.DropOff)
        {
            ModifyLoading(elevatorDestination.Capacity, ElevatorLoadingOptions.Remove);
        }
        else
        {
            ModifyLoading(elevatorDestination.Capacity, ElevatorLoadingOptions.Add);
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
   
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool ReachedThresholdFloor()
    {
        return CurrentFloor == HighestFloor || CurrentFloor == LowestFloor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task ActivateAsync()
    {
        return Task.Run(() =>
        {
            Thread elevatorThread = new Thread(Run);
            elevatorThread.IsBackground = true;
            elevatorThread.Start();
        });
    }
    
}