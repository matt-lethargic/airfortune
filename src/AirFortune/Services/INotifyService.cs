using AirFortune.Models;

namespace AirFortune.Services;

public interface INotifyService
{
    void ChangeTable(AirFortuneTable table);
    event EventHandler<AirFortuneTable>? TableChanged;
}