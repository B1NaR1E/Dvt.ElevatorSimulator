using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorControlSystem : IElevatorControlSystem
{
    private readonly IStrategy _schedulingStrategy;
    private List<IElevator> Elevators { get; }
    private readonly List<ElevatorCallRequest> _elevatorCalls;
    private readonly Dictionary<Guid, List<ElevatorCallRequest>> _elevatorJobs;

    public ElevatorControlSystem(IStrategy schedulingStrategy, List<IElevator> elevators)
    {
        _schedulingStrategy = schedulingStrategy;
        Elevators = elevators;
        _elevatorCalls = new List<ElevatorCallRequest>();
        _elevatorJobs = new Dictionary<Guid, List<ElevatorCallRequest>>();
        CreateElevatorQueues();
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

    private void CreateElevatorQueues()
    {
        foreach (var elevator in Elevators)
        {
            _elevatorJobs.Add(elevator.Id, new List<ElevatorCallRequest>());
        }
    }

    private Guid ProcessRequest(ElevatorCallRequest request)
    {
        return _schedulingStrategy.ProcessRequest(Elevators, request);
    }

    /// <summary>
    /// Emulates a elevator step.
    /// </summary>
    public void Step()
    {
        if (_elevatorCalls.Any())
        {
            var request = _elevatorCalls.First();
            var elevatorId = ProcessRequest(request);

            var elevator = Elevators.FirstOrDefault(e => e.Id == elevatorId);

            if (elevator is not null)
            {
                _elevatorJobs[elevatorId].Add(request);
                elevator.AddStop(request.OriginatingFloor);
                _elevatorCalls.Remove(request);
            }
        }

        Elevators.ForEach(e =>
        {
            //Unload passengers if possible
            if (e.State is State.Stopped or State.OverLimit && e.HasPassengers)
            {
                e.UnloadPassengers();
            }

            //Load passengers if possible
            if (e.State != State.OverLimit)
            {
                var loadPassengerJobs = _elevatorJobs[e.Id].Where(j => j.OriginatingFloor == e.CurrentFloor).ToList();

                foreach (var job in loadPassengerJobs)
                {
                    var passengersLoadedSuccessfully = e.LoadPassenger(job.TotalPassengers, job.DestinationFloor);

                    if (!passengersLoadedSuccessfully)
                        _elevatorCalls.Add(job);

                    _elevatorJobs[e.Id].Remove(job);
                }
            }
            
            //Moves elevator by one floor
            e.Move();
        });
    }

    public bool AnyOutstandingPickups()
    {
        return _elevatorCalls.Any() || Elevators.Any(e => e.Stops.Any() || e.HasPassengers);
    }
}