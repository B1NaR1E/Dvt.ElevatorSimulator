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

    public Direction Direction {
        get
        {
            if (CurrentFloor < DestinationFloor)
                return Direction.Up;
            else if (CurrentFloor == DestinationFloor && !Stops.Any())
                return Direction.Static;
            else
                return Direction.Down;
        }
    }
    
    private readonly IPassengerManager _passengerManager;
    private readonly IElevatorLogManager<ElevatorLog> _elevatorLogManager;

    public Elevator(IPassengerManager passengerManager, IElevatorLogManager<ElevatorLog> logManager)
    {
        Id = Guid.NewGuid();
        State = State.Stopped;
        CurrentFloor = 1;
        DestinationFloor = 1;
        
        _passengerManager = passengerManager;
        _elevatorLogManager = logManager;
        Stops = new List<int>();
    }

    public bool LoadPassenger(int totalPassengers, int destinationFloor)
    {
        for (int x = 0; x < totalPassengers; x++)
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
        if (Stops.Any())
            return Stops.MinBy(s => Math.Abs(s - CurrentFloor));
        else
            return DestinationFloor;
    }

    public void AddStop(int destinationFloor)
    {
        if(!Stops.Contains(destinationFloor))
            Stops.Add(destinationFloor);
    }

    public void Move()
    {
        DestinationFloor = GetDestination();

        if (DestinationFloor != CurrentFloor)
        {
            State = State.Moving;
            if (Direction == Direction.Up)
                MoveUp();
            else
                MoveDown();
        }

        if (CurrentFloor == DestinationFloor)
        {
            State = State.Stopped;
            Stops.Remove(CurrentFloor);
        }
    }


    private void MoveUp()
    {
        if (CurrentFloor != 10)
        {
            ++CurrentFloor;
            var message = $"Elevator moved up to floor: {CurrentFloor}.";
            CreateLog(LogType.MovementLog, message);
        }
    }

    private void MoveDown()
    {
        if (CurrentFloor != 1)
        {
            --CurrentFloor;
            var message = $"Elevator moved down to floor: {CurrentFloor}.";
            CreateLog(LogType.MovementLog, message);
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

        if (totalPassengersUnloaded != 0)
        {
            var message = $"Total passenger unloaded: {totalPassengersUnloaded} on floor: {CurrentFloor}.";
            CreateLog(LogType.UnloadingLog, message);
        }
        
        return totalPassengersUnloaded;
    }

    public List<ElevatorLog> GetElevatorLogs()
    {
        return _elevatorLogManager.GetAllLogs();
    }

    public int TotalPassengers()
    {
        return _passengerManager.CurrentPassengers();
    }

    private void CreateLog(LogType type, string message)
    {
        _elevatorLogManager.Log(new ElevatorLog(Id, type, CurrentFloor, DestinationFloor, Direction,
            _passengerManager.CurrentPassengers(), message));
    }
}