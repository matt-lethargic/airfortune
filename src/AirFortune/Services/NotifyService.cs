using AirFortune.Models;

namespace AirFortune.Services;

public class NotifyService: INotifyService
{
    public void SelectTable(AirFortuneTable table)
    {
        TableSelected?.Invoke(this, table);
    }

    public void UpdateTables()
    {
        TablesUpdated?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<AirFortuneTable>? TableSelected;
    public event EventHandler? TablesUpdated;
}