using AirFortune.Models;

namespace AirFortune.Services;

public interface INotifyService
{
    void SelectTable(AirFortuneTable table);
    void UpdateTables();
    event EventHandler<AirFortuneTable>? TableSelected;
    event EventHandler? TablesUpdated;
}