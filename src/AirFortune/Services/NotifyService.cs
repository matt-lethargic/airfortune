using AirFortune.Models;

namespace AirFortune.Services;

public class NotifyService: INotifyService
{
    public void ChangeTable(AirFortuneTable table)
    {
        TableChanged?.Invoke(this, table);
    }

    public event EventHandler<AirFortuneTable>? TableChanged;
}