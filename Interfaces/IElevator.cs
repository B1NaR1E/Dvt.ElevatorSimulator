using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevator
{
    Guid Id { get; }
    int CurrentFloor { get; set; }
    int DestinationFloor { get; set; }
    Direction Direction { get; }
    State State { get; }
    bool HasCapacity { get; }
    bool LoadPassenger(int totalPassengers, int destinationFloor);
    int UnloadPassengers();
    void Move();
    List<ElevatorLog> GetElevatorLogs();

}