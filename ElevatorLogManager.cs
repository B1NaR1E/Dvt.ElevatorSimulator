using Dvt.ElevatorSimulator.Interfaces;
using Dvt.ElevatorSimulator.Models;

namespace Dvt.ElevatorSimulator;

public class ElevatorLogManager : IElevatorLogManager<ElevatorLog>
{
    private readonly List<ElevatorLog> _logs = new();

    public void Log(ElevatorLog log)
    {
        _logs.Add(log);
    }

    public List<ElevatorLog> GetAllLogs()
    {
        return _logs;
    }
}