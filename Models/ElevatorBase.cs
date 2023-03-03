using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public abstract class ElevatorBase<TPassenger> : IElevator<TPassenger>
    where TPassenger : Passenger
{
    public Guid Id { get; }
    public int CurrentFloor { get; protected set; }
    public int DestinationFloor { get; private set; }
    public List<int> Stops { get; }
    public State State { get; protected set; }
    public bool HasPassengers => PassengerManager.HasPassengers;

    protected IPassengerManager<TPassenger> PassengerManager { get; }

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
        
        PassengerManager = passengerManager;
        Stops = new List<int>();
    }

    bool IElevator<TPassenger>.LoadPassenger(List<TPassenger> passengers)
    {
        return LoadPassenger(passengers);
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

    List<TPassenger> IElevator<TPassenger>.UnloadPassengers()
    {
        return UnloadPassengers();
    }

    public int TotalPassengers()
    {
        return PassengerManager.TotalPassenger();
    }
    protected abstract void MoveUp();
    protected abstract void MoveDown();
    protected abstract int GetDestination();
    protected abstract bool LoadPassenger(List<TPassenger> passengers);
    protected abstract List<TPassenger> UnloadPassengers();
}