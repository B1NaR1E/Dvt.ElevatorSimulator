using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IPassengerManager
{
    bool HasCapacity { get; }
    bool OverPassengerLimit { get; }
    int CurrentCapacity();
    void LoadPassenger(Passenger passenger);
    int UnloadPassengers(int floorNumber);
    int UnloadOverLimitPassengers(int floorNumber);
    int CurrentPassengers();

}