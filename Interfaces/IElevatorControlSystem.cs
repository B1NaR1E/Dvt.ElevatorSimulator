using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevatorControlSystem
{
    Elevator GetStatus(int elevatorId);
    void Update(int elevatorId, int floorNumber, int goalFloorNumber);
    void Pickup(int pickupFloor, int destinationFloor);
    void Step();
    bool AnyOutstandingPickups();
}