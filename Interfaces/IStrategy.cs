using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Interfaces;

public interface IStrategy<TPassenger> where TPassenger : IPassenger
{
    Guid ProcessRequest(List<IElevator<TPassenger>> elevators, ElevatorCallRequest request);
}