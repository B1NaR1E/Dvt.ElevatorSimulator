using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public abstract class ElevatorBase<TPassenger> : IElevator<TPassenger>
    where TPassenger : IPassenger
{
    public Guid Id { get; }
    public int CurrentFloor { get; protected set; }
    public int DestinationFloor { get; private set; }
    public List<int> Stops { get; }
    public State State { get; private set; }
    
    public bool HasCapacity => _passengerManager.HasCapacity;
    public bool HasPassengers => _passengerManager.HasPassengers;
    
    private readonly IPassengerManager<TPassenger> _passengerManager;

    public Direction Direction {
        get
        {
            if (CurrentFloor < DestinationFloor)
                return Direction.Up;
            if (CurrentFloor == DestinationFloor && !Stops.Any())
                return Direction.Static;
            
            return Direction.Down;
        }
    }

    protected ElevatorBase(IPassengerManager<TPassenger> passengerManager)
    {
        Id = Guid.NewGuid();
        State = State.Stopped;
        CurrentFloor = 1;
        DestinationFloor = 1;
        
        _passengerManager = passengerManager;
        Stops = new List<int>();
    }

    public bool LoadPassenger(List<TPassenger> passengers)
    {
        foreach (var passenger in passengers)
        {
            _passengerManager.LoadPassenger(passenger);
        }

        var destinationFloor = passengers.First().DestinationFloor;
        
        if (_passengerManager.OverPassengerLimit)
        {
            State = State.OverLimit;
            return false;
        }

        AddStop(destinationFloor);
        return true;
    }

    public void AddStop(int destinationFloor)
    {
        if(!Stops.Contains(destinationFloor) && destinationFloor != CurrentFloor)
            Stops.Add(destinationFloor);
    }

    public void Move()
    {
        DestinationFloor = GetDestination();

        if (DestinationFloor == CurrentFloor || State == State.OverLimit) 
            return;
        
        State = State.Moving;
        if (Direction == Direction.Up)
            MoveUp();
        else
            MoveDown();

        if (CurrentFloor != DestinationFloor) 
            return;
        
        State = State.Stopped;
        Stops.Remove(CurrentFloor);
    }

    public int UnloadPassengers()
    {
        int totalPassengersUnloaded;
        if (_passengerManager.OverPassengerLimit)
        {
            State = State.Stopped;
            totalPassengersUnloaded = _passengerManager.UnloadOverLimitPassengers(CurrentFloor);
        }
        else
        {
            totalPassengersUnloaded = _passengerManager.UnloadPassengers(CurrentFloor);
        }

        return totalPassengersUnloaded;
    }

    public int TotalPassengers()
    {
        return _passengerManager.TotalPassenger();
    }
    
    protected abstract void MoveUp();

    protected abstract void MoveDown();
    
    protected abstract int GetDestination();
}