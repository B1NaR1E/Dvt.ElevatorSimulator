using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public class Elevator : IElevator
{
    public Guid Id { get; }
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }
    public List<int> Stops { get; }
    public State State { get; private set; }
    
    public bool HasCapacity => _passengerManager.HasCapacity;
    public bool HasPassengers => _passengerManager.CurrentPassengers() > 0;
    
    private readonly IPassengerManager _passengerManager;
    private readonly int _totalFloors;
    
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

    public Elevator(IPassengerManager passengerManager, int totalFloors)
    {
        Id = Guid.NewGuid();
        State = State.Stopped;
        CurrentFloor = 1;
        DestinationFloor = 1;
        
        _passengerManager = passengerManager;
        _totalFloors = totalFloors;
        Stops = new List<int>();
    }

    public bool LoadPassenger(int totalPassengers, int destinationFloor)
    {
        for (var x = 0; x < totalPassengers; x++)
        {
            _passengerManager.LoadPassenger(new Passenger(CurrentFloor, destinationFloor));
        }

        if (_passengerManager.OverPassengerLimit)
        {
            State = State.OverLimit;
            return false;
        }

        AddStop(destinationFloor);
        return true;
    }

    private int GetDestination()
    {
        return Stops.Any() ? Stops.MinBy(s => Math.Abs(s - CurrentFloor)) : DestinationFloor;
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


    private void MoveUp()
    {
        if (CurrentFloor != _totalFloors)
        {
            ++CurrentFloor;
        }
    }

    private void MoveDown()
    {
        if (CurrentFloor != 1)
        {
            --CurrentFloor;
        }
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
        return _passengerManager.CurrentPassengers();
    }
}