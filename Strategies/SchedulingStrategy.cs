using Dvt.ElevatorSimulator.Enums;
using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator.Strategies;

public class SchedulingStrategy : IStrategy
{
    public Guid ProcessRequest(List<IElevator> elevators, ElevatorCallRequest request)
    {
        IElevator? selectedElevator = null;

        switch (request.Direction)
        {
            case Direction.Up:
            {
                var elevatorsGoingUp = elevators.Where(e =>
                        e.Direction == request.Direction &&
                        e.CurrentFloor <= request.OriginatingFloor &&
                        e.HasCapacity &&
                        e.State != State.OverLimit)
                    .ToList();

                if (elevatorsGoingUp.Any())
                    selectedElevator = GetClosestElevator(elevatorsGoingUp, request.OriginatingFloor);
                break;
            }
            case Direction.Down:
            {
                var elevatorsGoingDown = elevators.Where(e =>
                        e.Direction == request.Direction &&
                        e.CurrentFloor >= request.OriginatingFloor &&
                        e.HasCapacity &&
                        e.State != State.OverLimit)
                    .ToList();

                if (elevatorsGoingDown.Any())
                    selectedElevator = GetClosestElevator(elevatorsGoingDown, request.OriginatingFloor);
                break;
            }
        }

        if (selectedElevator != null)
            return selectedElevator.Id;

        var staticElevators = elevators.Where(e => e.Direction == Direction.Static && e.State != State.OverLimit)
            .ToList();

        if (staticElevators.Any())
            selectedElevator = GetClosestElevator(staticElevators, request.OriginatingFloor);


        return selectedElevator?.Id ?? Guid.Empty;
    }

    private static IElevator? GetClosestElevator(List<IElevator> elevators, int originatingFloor)
    {
        return elevators.Aggregate((x, y) =>
            Math.Abs(x.CurrentFloor - originatingFloor) <
            Math.Abs(y.CurrentFloor - originatingFloor) ? x : y);
    }
}