using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevatorControlSystem
{
    void Pickup(int pickupFloor, int destinationFloor, int totalPassengers);
    void Step();
    bool AnyOutstandingPickups();
    int GetTotalRequests();
    int GetTotalPassengers();
}