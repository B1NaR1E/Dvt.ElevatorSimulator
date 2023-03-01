using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorPassengerManager : IPassengerManager
{
    private readonly int _maximumPassengers = 5;
    private List<Passenger> Passengers { get; }
    public bool HasCapacity => Passengers.Count < _maximumPassengers;
    public bool OverPassengerLimit => Passengers.Count > _maximumPassengers;

    public ElevatorPassengerManager()
    {
        Passengers = new List<Passenger>();
    }

    public ElevatorPassengerManager(int maximumPassengers)
    {
        _maximumPassengers = maximumPassengers;
        Passengers = new List<Passenger>();
    }

    public void LoadPassenger(Passenger passenger)
    {
        Passengers.Add(passenger);
    }

    public int UnloadPassengers(int floorNumber)
    {
        return Passengers.RemoveAll(p => p.DestinationFloor == floorNumber);
    }

    public int CurrentCapacity()
    {
        return _maximumPassengers - Passengers.Count;
    }

    public int CurrentPassengers()
    {
        return Passengers.Count;
    }
}