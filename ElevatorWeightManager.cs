using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

//Not tested at all but it should work.
public class ElevatorWeightManager : IPassengerManager<PassengerWithWeight>
{
    private readonly decimal _weightLimit = 500;

    public ElevatorWeightManager()
    {
        _passengers = new List<PassengerWithWeight>();
    }

    public bool HasPassengers => _passengers.Count > 0;
    public bool OverPassengerLimit => TotalWeight() > _weightLimit;
    private readonly List<PassengerWithWeight> _passengers;
    
    public void LoadPassenger(PassengerWithWeight passenger)
    {
        _passengers.Add(passenger);
    }

    public int UnloadPassengers(int floorNumber)
    {
        return _passengers.RemoveAll(p => p.DestinationFloor == floorNumber);
    }

    public int UnloadOverLimitPassengers(int floorNumber)
    {
        return _passengers.RemoveAll(p => p.OriginatingFloor == floorNumber);
    }

    public int TotalPassenger()
    {
        return _passengers.Count;
    }

    private decimal TotalWeight()
    {
        return _passengers.Sum(p => p.Weight);
    }
}