using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorControlSystem : IElevatorControlSystem
{
    private List<Elevator> Elevators { get; }
    //private List<Passenger> WaitingPassengers { get; }
    private List<ElevatorCallRequest> _elevatorCalls;
    
    private ElevatorLogManager Logger { get; }

    public ElevatorControlSystem(List<Elevator> elevators)
    {
        Logger = new ElevatorLogManager();
        Elevators = elevators;
        //WaitingPassengers = new List<Passenger>();
        _elevatorCalls = new List<ElevatorCallRequest>();
    }

    public Elevator GetStatus(Guid elevatorId)
    {
        return Elevators.First(e => e.Id == elevatorId);
    }

    public int GetTotalPassengers()
    {
        return _elevatorCalls.Sum(e => e.TotalPassengers);
    }

    public int GetTotalRequests()
    {
        return _elevatorCalls.Count;
    }

    public void Pickup(int pickupFloor, int destinationFloor, int totalPassengers)
    {
        //WaitingPassengers.Add(new Passenger(pickupFloor, destinationFloor));
        _elevatorCalls.Add(new ElevatorCallRequest(pickupFloor, destinationFloor, totalPassengers));
    }

    private Guid ProcessRequest(ElevatorCallRequest request)
    {
        Elevator? selectedElevator = null;

        if (request.Direction == Direction.Up)
        {
            var elevators = Elevators.Where(e => 
            e.Direction == request.Direction &&
            e.CurrentFloor <= request.OriginatingFloor && 
            e.HasCapacity && e.State != State.OverLimit)
                .ToList();

            if (elevators.Any())
                selectedElevator = GetClosestElevator(elevators, request.OriginatingFloor);
        }

        if (request.Direction == Direction.Down)
        {
            var elevators = Elevators.Where(e => 
            e.Direction == request.Direction && 
            e.CurrentFloor >= request.OriginatingFloor && 
            e.HasCapacity && 
            e.State != State.OverLimit)
                .ToList();

            if (elevators.Any())
                selectedElevator = GetClosestElevator(elevators, request.OriginatingFloor);
        }

        if(selectedElevator == null)
        {
            var elevators = Elevators.Where(e => e.Direction == Direction.Static && e.).ToList();

            if (elevators.Any())
                selectedElevator = GetClosestElevator(elevators, request.OriginatingFloor);
        }

        return selectedElevator?.Id ?? Guid.Empty;
    }

    private Elevator? GetClosestElevator(List<Elevator> elevators, int originatingFloor)
    {
        return elevators.Aggregate((x, y) =>
            Math.Abs(originatingFloor - x.CurrentFloor) <
            Math.Abs(originatingFloor - y.CurrentFloor) ? x : y);
    }

    public void Step()
    {
        //if (_elevatorCalls.Any())
        //{
        //    var request = _elevatorCalls.Dequeue();
        //    var elevatorId = ProcessRequest(request);

        //    var elevator = Elevators.FirstOrDefault(e => e.Id == elevatorId);

        //    if(elevator is null)
        //        _elevatorCalls.Enqueue(request); 
        //    else
        //        elevator!.AddStop(request.OriginatingFloor);
        //}

        //Elevators.ForEach(e =>
        //{
        //    e
        //    e.Move();
        //});
        foreach (var elevator in Elevators.Where(e => e.State == State.Stopped && e.HasPassengers))
        {
            elevator.UnloadPassengers();
        }

        var floorsWithAvailableElevators = Elevators
            .Where(e => e.HasCapacity && e.State != State.OverLimit).GroupBy(e => e.CurrentFloor)
            .OrderBy(e => e.Key)
            .ToList();

        foreach (var elevators in floorsWithAvailableElevators)
        {
            foreach (var elevator in elevators.ToList())
            {
                var call =
                    _elevatorCalls
                    .Where(wp =>
                        (wp.OriginatingFloor == elevator.CurrentFloor && wp.Direction == elevator.Direction) ||
                        (wp.OriginatingFloor == elevator.CurrentFloor && !elevator.Stops.Any()))
                    .FirstOrDefault();

                if (call is null)
                    continue;

                var loaded = elevator.LoadPassenger(call.TotalPassengers, call.DestinationFloor);

                if (loaded)
                    _elevatorCalls.Remove(call);
            }
        }

        Elevators.ForEach(e =>
        {
            if (e.State == State.OverLimit)
                e.UnloadPassengers();

            if ((e.Direction == Direction.Static) && _elevatorCalls.Any())
            {
                // Lots of optimization could be done here, perhaps?
                e.DestinationFloor = _elevatorCalls
                    .GroupBy(r => new { r.OriginatingFloor })
                    .OrderBy(g => g.Count())
                    .First().Key.OriginatingFloor;
            }


            e.Move();
        });
    }

    public bool AnyOutstandingPickups()
    {
        return _elevatorCalls.Any() || Elevators.Any(e => e.Stops.Any() || Elevators.Any(e => e.HasPassengers));
    }

    public List<ElevatorLog> GetAllLogs()
    {
        return Logger.GetAllLogs();
    }
}