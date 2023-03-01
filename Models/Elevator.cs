using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;

namespace Dvt.ElevatorSimulator.Models;

public class Elevator : IElevator
{
    public Guid Id { get; }
    public int CurrentFloor { get; set; }
    public int DestinationFloor { get; set; }
    public List<ElevatorCallRequest> Requests { get; }
    public State State { get; private set; }
    public ElevatorCallRequest? CurrentRequest { get; private set; }
    public bool HasCapacity => _passengerManager.HasCapacity;
    public bool HasPassengers => _passengerManager.CurrentPassengers() > 0;
    
    public Direction Direction =>
        (CurrentFloor == 1 || DestinationFloor > CurrentFloor) && CurrentFloor != 10
            ? Direction.Up
            : Direction.Down;
    
    private readonly IPassengerManager _passengerManager;
    private readonly IElevatorLogManager<ElevatorLog> _elevatorLogManager;

    public Elevator(IPassengerManager passengerManager, IElevatorLogManager<ElevatorLog> logManager)
    {
        Id = Guid.NewGuid();
        State = State.Stopped;
        CurrentFloor = 1;
        DestinationFloor = 1;
        Requests = new List<ElevatorCallRequest>();
        
        _passengerManager = passengerManager;
        _elevatorLogManager = logManager;
        Requests = new List<ElevatorCallRequest>();
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
        return true;
    }

    private ElevatorCallRequest? GetCurrentRequest()
    {
        var request = Requests.MinBy(s => Math.Abs(s.DestinationFloor - CurrentFloor));
        return request;
    }

    public void AddRequest(ElevatorCallRequest callRequest)
    {
        Requests.Add(callRequest);
    }

    public void Move()
    {
        CurrentRequest = GetCurrentRequest();
        
        DestinationFloor = CurrentRequest?.DestinationFloor ?? DestinationFloor;

        if (DestinationFloor != CurrentFloor)
        {
            State = State.Moving;
            if (Direction == Direction.Up)
                MoveUp();
            else
                MoveDown();

            if (CurrentFloor == DestinationFloor || Requests.Any(r => r.OriginatingFloor == CurrentFloor))
            {
                State = State.Stopped;
                
                var loadingTask = Requests.FirstOrDefault(r => r.OriginatingFloor == CurrentFloor);
                if (loadingTask is not null)
                {
                    LoadPassenger(loadingTask.TotalPassengers, loadingTask.DestinationFloor);
                }
                
                if(Requests.Any(r => r.DestinationFloor == DestinationFloor))
                {
                    UnloadPassengers();
                    Requests.RemoveAll(r => r.DestinationFloor == CurrentFloor);
                }
            }
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
        if (_passengerManager.OverPassengerLimit)
            State = State.Stopped;
        
        int totalPassengersUnloaded = _passengerManager.UnloadPassengers(CurrentFloor);;

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

    private void CreateLog(LogType type, string message)
    {
        _elevatorLogManager.Log(new ElevatorLog(Id, type, CurrentFloor, DestinationFloor, Direction,
            _passengerManager.CurrentPassengers(), message));
    }
}