namespace Dvt.ElevatorSimulator.Interfaces;

public interface IElevatorLogManager<TLog>
{
    void Log(TLog log);
    List<TLog> GetAllLogs();
}