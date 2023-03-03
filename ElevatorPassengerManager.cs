using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorPassengerManager : IPassengerManager<Passenger>
{
    private readonly int _maximum;
    private readonly List<Passenger> _passengers;
    public bool HasPassengers => _passengers.Count > 0;
    public bool OverPassengerLimit => TotalPassenger() > _maximum;

    public ElevatorPassengerManager(int maximum)
    {
        _maximum = maximum;
        _passengers = new List<Passenger>();
    }

    public void LoadPassenger(Passenger passenger)
    {
        _passengers.Add(passenger);
    }

    public IEnumerable<Passenger> UnloadPassengers(int floorNumber)
    {
        var passengers = _passengers.Where(p => p.DestinationFloor == floorNumber);
        _passengers.RemoveAll(p => p.DestinationFloor == floorNumber);
        return passengers.ToList();
    }

    public int TotalPassenger()
    {
        return _passengers.Count;
    }

    public IEnumerable<Passenger> UnloadOverLimitPassengers(int floorNumber)
    {
        var passengers = _passengers.Where(p => p.OriginatingFloor == floorNumber);
        _passengers.RemoveAll(p => p.OriginatingFloor == floorNumber);
        return passengers.ToList();
    }
}