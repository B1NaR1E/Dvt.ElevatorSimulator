using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorControlSystem<TPassenger> : IElevatorControlSystem where TPassenger : Passenger
{
    private readonly IStrategy<TPassenger> _schedulingStrategy;
    private List<IElevator<TPassenger>> Elevators { get; }
    private readonly List<ElevatorCallRequest> _elevatorCalls;
    private readonly Dictionary<Guid, List<ElevatorCallRequest>> _elevatorJobs;

    public ElevatorControlSystem(IStrategy<TPassenger> schedulingStrategy, List<IElevator<TPassenger>> elevators)
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
    
    //Create job queues for each elevator. Uses elevator's Id as the queue Id
    //I can remove the queues completely if I change UnloadPassengers() to return List<TPassenger> instead an integer.
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
                //If elevator is found the system loads the request on the elevator job queue.
                _elevatorJobs[elevatorId].Add(request);
                
                //Adds the floor of the request to the elevator's stops
                elevator.AddStop(request.OriginatingFloor);
                
                //If request is handed off to the elevator the system removes the request from it's queue.
                _elevatorCalls.Remove(request);
            }
        }

        Elevators.ForEach(e =>
        {
            //Unload passengers if it has any or if the elevator's state is OverLimit it will unload the passengers that where unsuccessfully loaded.
            if ((e.State is State.Stopped or State.OverLimit) && e.HasPassengers)
            {
                e.UnloadPassengers();
            }

            //Load passengers if the elevator has passengers to load on this floor and elevator state is not OverLimit
            if (e.State != State.OverLimit)
            {
                var loadPassengerJobs = _elevatorJobs[e.Id].Where(j => j.OriginatingFloor == e.CurrentFloor).ToList();

                foreach (var job in loadPassengerJobs)
                {
                    var passengers = Enumerable.Range(0, job.TotalPassengers)
                        .Select(_ =>
                            new Passenger(job.OriginatingFloor, job.DestinationFloor) as TPassenger)
                        .ToList();
                    var passengersLoadedSuccessfully = e.LoadPassenger(passengers!);

                    //if passenger are loaded but the elevator is over its passenger limit it will send the uncompleted 
                    //request back to the manager to reprocess and set elevator state to OverLimit.
                    if (!passengersLoadedSuccessfully)
                        _elevatorCalls.Add(job);

                    //Removes any jobs it could load successfully or it could not complete.
                    _elevatorJobs[e.Id].Remove(job);
                }
            }
            
            //Moves elevator to next floor
            e.Move();
        });
    }

    public bool AnyOutstandingPickups()
    {
        return _elevatorCalls.Any() || Elevators.Any(e => e.Stops.Any() || e.HasPassengers);
    }
}