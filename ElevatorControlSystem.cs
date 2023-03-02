using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorControlSystem : IElevatorControlSystem
{
    private List<Elevator> Elevators { get; }
    private List<ElevatorCallRequest> _elevatorCalls;
    private Dictionary<Guid, List<ElevatorCallRequest>> elevatorJobs;
    
    private ElevatorLogManager Logger { get; }

    public ElevatorControlSystem(List<Elevator> elevators)
    {
        Logger = new ElevatorLogManager();
        Elevators = elevators;
        _elevatorCalls = new List<ElevatorCallRequest>();
        elevatorJobs = new Dictionary<Guid, List<ElevatorCallRequest>>();
        CreateElevatorQueses();
    }

    private void CreateElevatorQueses()
    {
        foreach (var elevator in Elevators)
        {
            elevatorJobs.Add(elevator.Id, new List<ElevatorCallRequest>());
        }
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
            e.HasCapacity && 
            e.State != State.OverLimit && 
            elevatorJobs[e.Id].Count !> 3)
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
            e.State != State.OverLimit &&
            elevatorJobs[e.Id].Count! > 3)
                .ToList();

            if (elevators.Any())
                selectedElevator = GetClosestElevator(elevators, request.OriginatingFloor);
        }

        if(selectedElevator == null)
        {
            var elevators = Elevators.Where(e => e.Direction == Direction.Static && e.State != State.OverLimit).ToList();

            if (elevators.Any())
                selectedElevator = GetClosestElevator(elevators, request.OriginatingFloor);
        }

        return selectedElevator?.Id ?? Guid.Empty;
    }

    private Elevator? GetClosestElevator(List<Elevator> elevators, int originatingFloor)
    {
        return elevators.Aggregate((x, y) =>
            Math.Abs(x.CurrentFloor - originatingFloor) <
            Math.Abs(y.CurrentFloor - originatingFloor) ? x : y);
    }

    public void Step()
    {
        if (_elevatorCalls.Any())
        {
            var request = _elevatorCalls.First();
            var elevatorId = ProcessRequest(request);

            var elevator = Elevators.FirstOrDefault(e => e.Id == elevatorId);

            if (elevator is not null)
            {
                elevatorJobs[elevatorId].Add(request);
                elevator.AddStop(request.OriginatingFloor);
                _elevatorCalls.Remove(request);
            }
        }

        Elevators.ForEach(e =>
        {
            if(e.State == State.OverLimit)
            {
                e.UnloadPassengers();
            }

            if(e.CurrentFloor == e.DestinationFloor)
            {
                if((e.State == State.Stopped && e.HasPassengers))
                {
                    e.UnloadPassengers();
                }

                var loadPassengerJobs = elevatorJobs[e.Id].Where(j => j.OriginatingFloor == e.CurrentFloor).ToList();

                foreach (var job in loadPassengerJobs)
                {
                    var passengersLoadedSuccessfully = e.LoadPassenger(job.TotalPassengers, job.DestinationFloor);
                    if (passengersLoadedSuccessfully)
                        elevatorJobs[e.Id].Remove(job);
                    else
                    {
                        elevatorJobs[e.Id].Remove(job);
                        _elevatorCalls.Add(job);
                    }
                }
            }
            e.Move();
        });

        //var floorsWithAvailableElevators = Elevators
        //    .Where(e => e.HasCapacity && e.State != State.OverLimit).GroupBy(e => e.CurrentFloor)
        //    .OrderBy(e => e.Key)
        //    .ToList();

        //foreach (var elevator in Elevators.Where(e => (e.State == State.Stopped && e.CurrentFloor == e.DestinationFloor && e.HasPassengers) || e.State == State.OverLimit))
        //{
        //    elevator.UnloadPassengers();
        //}

        //foreach (var elevators in floorsWithAvailableElevators)
        //{
        //    foreach (var elevator in elevators.ToList())
        //    {
        //        var call =
        //            _elevatorCalls
        //            .Where(wp =>
        //                (wp.OriginatingFloor == elevator.CurrentFloor && wp.Direction == elevator.Direction) ||
        //                (wp.OriginatingFloor == elevator.CurrentFloor && !elevator.Stops.Any()))
        //            .FirstOrDefault();

        //        if (call is null)
        //            continue;

        //        var loaded = elevator.LoadPassenger(call.TotalPassengers, call.DestinationFloor);

        //        if (loaded)
        //        {
        //            elevatorJobs[elevator.Id.ToString()].Add(call);
        //            _elevatorCalls.Remove(call);
        //        }
        //    }
        //}

        //Elevators.ForEach(e =>
        //{
        //    if ((e.Direction == Direction.Static) && _elevatorCalls.Any())
        //    {
        //        Lots of optimization could be done here, perhaps?
        //        e.DestinationFloor = _elevatorCalls
        //            .GroupBy(r => new { r.OriginatingFloor })
        //            .OrderBy(g => g.Count())
        //            .First().Key.OriginatingFloor;
        //    }


        //    e.Move();

        //    if (e.State == State.OverLimit)
        //        e.UnloadPassengers();
        //});
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