using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorControlSystem : IElevatorControlSystem
{
    private List<Elevator> Elevators { get; }
    //private List<Passenger> WaitingPassengers { get; }
    private Queue<ElevatorCallRequest> _elevatorCalls;
    
    private ElevatorLogManager Logger { get; }

    public ElevatorControlSystem(List<Elevator> elevators)
    {
        Logger = new ElevatorLogManager();
        Elevators = elevators;
        //WaitingPassengers = new List<Passenger>();
        _elevatorCalls = new Queue<ElevatorCallRequest>();
    }
    
    public Elevator GetStatus(Guid elevatorId)
    {
        return Elevators.First(e => e.Id == elevatorId);
    }

    public void Pickup(int pickupFloor, int destinationFloor, int totalPassengers)
    {
        //WaitingPassengers.Add(new Passenger(pickupFloor, destinationFloor));
        _elevatorCalls.Enqueue(new ElevatorCallRequest(pickupFloor, destinationFloor, totalPassengers));
    }

    private Guid ProcessRequest(ElevatorCallRequest request)
    {
        var elevators = Elevators.Where(e =>
            e is { State: State.Stopped, HasPassengers: false, Requests.Count: < 5 } || (e.State == State.Moving && e.Direction == request.Direction && e.HasCapacity && e.Requests.Count < 5)) 
            .OrderBy(e => e.CurrentFloor)
            .ToList();

        // if (!elevators.Any())
        // {
        //     elevators = Elevators.Where(e => e is { State: State.Stopped, HasCapacity: true }).ToList();
        // }

        if (!elevators.Any())
        {
            //elevators = Elevators.Where(e => e is { State: State.Stopped, HasCapacity: true }).ToList();
            return Guid.Empty;
        }
        
        var elevator = elevators.Aggregate((x, y) =>
            Math.Abs(x.CurrentFloor - request.OriginatingFloor) < 
            Math.Abs(y.CurrentFloor - request.OriginatingFloor) ? x : y);

        return elevator.Id;
    }

    public void Step()
    {
        if (_elevatorCalls.Any())
        {
            var request = _elevatorCalls.Dequeue();
            var elevatorId = ProcessRequest(request);

            var elevator = Elevators.FirstOrDefault(e => e.Id == elevatorId);
            
            if(elevator is null)
                _elevatorCalls.Enqueue(request); 
            else
                elevator!.AddRequest(request);

            Elevators.ForEach(e =>
            {
                e.Move();
            });
        }
        // foreach (var elevator in Elevators.Where(e => e.State == State.Stopped))
        // {
        //     elevator.UnloadPassengers();
        // }
        //
        // var floorsWithAvailableElevators = Elevators
        //     .Where(e => e.HasCapacity).GroupBy(e => e.CurrentFloor)
        //     .OrderBy(e => e.Key)
        //     .ToList();
        //
        // foreach (var elevators in floorsWithAvailableElevators)
        // {
        //     foreach (var elevator in elevators.ToList())
        //     {
        //         var passengers = 
        //             WaitingPassengers
        //             .Where(wp =>
        //                 (wp.OriginatingFloor == elevator.CurrentFloor && wp.Direction == elevator.Direction) || 
        //                 (wp.OriginatingFloor == elevator.CurrentFloor && !elevator.Stops.Any()))
        //             .ToList();
        //
        //         if (!passengers.Any())
        //             continue;
        //
        //         foreach (var passenger in passengers)
        //         {
        //             var result = elevator.LoadPassenger(passenger);
        //             if (result)
        //                 WaitingPassengers.Remove(passenger);
        //             else
        //                 break;
        //         }
        //     }
        // }
        //
        // Elevators.ForEach(e =>
        // {
        //     if ((e.DestinationFloor == e.CurrentFloor && !e.Stops.Any()) && WaitingPassengers.Any())
        //     {
        //         // Lots of optimization could be done here, perhaps?
        //         e.DestinationFloor = WaitingPassengers
        //             .GroupBy(r => new { r.OriginatingFloor })
        //             .OrderBy(g => g.Count())
        //             .First().Key.OriginatingFloor;
        //     }
        //
        //     e.Move();
        // });
    }

    public bool AnyOutstandingPickups()
    {
        return _elevatorCalls.Any() || Elevators.Any(e => e.Requests.Any());
    }

    public List<ElevatorLog> GetAllLogs()
    {
        return Logger.GetAllLogs();
    }
}