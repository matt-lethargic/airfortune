using AirFortune.Models;

namespace AirFortune.Services;

public interface ITableService
{
    void ChangeTable(AirFortuneTable table);
    Task<IEnumerable<AirFortuneTable>> GetTables();
}