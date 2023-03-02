using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IStrategy
{
    Guid ProcessRequest(List<IElevator> elevators, ElevatorCallRequest request);
}