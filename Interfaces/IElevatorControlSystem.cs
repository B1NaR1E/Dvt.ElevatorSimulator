using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevatorControlSystem
{
    Elevator GetStatus(Guid elevatorId);
    void Pickup(int pickupFloor, int destinationFloor, int totalPassengers);
    void Step();
    bool AnyOutstandingPickups();
    int GetTotalRequests();
    int GetTotalPassengers();
    List<ElevatorLog> GetAllLogs();
}