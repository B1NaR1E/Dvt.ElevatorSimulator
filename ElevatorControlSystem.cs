using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorControlSystem : IElevatorControlSystem
{
    private List<Elevator> Elevators { get; set; }
    private List<Passenger> WaitingRiders { get; set; }

    public ElevatorControlSystem(int numberOfElevators)
    {
        Elevators = Enumerable.Range(0, numberOfElevators).Select(eid => new Elevator(eid)).ToList();
        WaitingRiders = new List<Passenger>();
    }


    public Elevator GetStatus(int elevatorId)
    {
        return Elevators.First(e => e.Id == elevatorId);
    }

    public void Update(int elevatorId, int floorNumber, int goalFloorNumber)
    {
        UpdateElevator(elevatorId, e =>
        {
            e.CurrentFloor = floorNumber;
            e.DestinationFloor = goalFloorNumber;
        });
    }

    public void Pickup(int pickupFloor, int destinationFloor)
    {
        WaitingRiders.Add(new Passenger(pickupFloor, destinationFloor));
    }

    private void UpdateElevator(int elevatorId, Action<Elevator> update)
    {
        Elevators = Elevators.Select(e =>
        {
            if (e.Id == elevatorId) update(e);
            return e;
        }).ToList();
    }

    public void Step()
    {
        var busyElevatorIds = new List<int>();

        // unload elevators
        Elevators = Elevators.Select(e =>
        {
            var disembarkingRiders = e.Passengers.Where(r => r.DestinationFloor == e.CurrentFloor).ToList();
            if (disembarkingRiders.Any())
            {
                busyElevatorIds.Add(e.Id);
                e.Passengers = e.Passengers.Where(r => r.DestinationFloor != e.CurrentFloor).ToList();
            }

            return e;
        }).ToList();

        // Embark passengers to available elevators
        WaitingRiders.GroupBy(r => new { r.OriginatingFloor, r.Direction }).ToList().ForEach(waitingFloor =>
        {
            var availableElevator =
                Elevators.FirstOrDefault(
                    e =>
                        e.CurrentFloor == waitingFloor.Key.OriginatingFloor &&
                        (e.Direction == waitingFloor.Key.Direction || !e.Passengers.Any()));
            if (availableElevator != null)
            {
                busyElevatorIds.Add(availableElevator.Id);
                var embarkingPassengers = waitingFloor.ToList();
                UpdateElevator(availableElevator.Id, e => e.Passengers.AddRange(embarkingPassengers.ToList()));
                WaitingRiders = WaitingRiders.Where(r => embarkingPassengers.All(er => er.Id != r.Id)).ToList();
            }
        });


        Elevators.ForEach(e =>
        {
            var isBusy = busyElevatorIds.Contains(e.Id);
            int destinationFloor;
            if (e.Passengers.Any())
            {
                var closestDestinationFloor =
                    e.Passengers.OrderBy(r => Math.Abs(r.DestinationFloor - e.CurrentFloor))
                        .First()
                        .DestinationFloor;
                destinationFloor = closestDestinationFloor;
            }
            else if (e.DestinationFloor == e.CurrentFloor && WaitingRiders.Any())
            {
                // Lots of optimization could be done here, perhaps?
                destinationFloor = WaitingRiders.GroupBy(r => new { r.OriginatingFloor }).OrderBy(g => g.Count())
                    .First().Key.OriginatingFloor;
            }
            else
            {
                destinationFloor = e.DestinationFloor;
            }

            var floorNumber = isBusy
                ? e.CurrentFloor
                : e.CurrentFloor + (destinationFloor > e.CurrentFloor ? 1 : -1);

            Update(e.Id, floorNumber, destinationFloor);
        });
    }

    public bool AnyOutstandingPickups()
    {
        return WaitingRiders.Any();
    }
}