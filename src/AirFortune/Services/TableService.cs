using AirFortune.Models;
using Blazored.LocalStorage;

namespace AirFortune.Services
{
    public class TableService : ITableService
    {
        private const string TablesStorageKey = "tables";

        private readonly ILocalStorageService _localStorageService;
        private readonly INotifyService _notifyService;
        private AirFortuneTable? _selectedTable;

        public TableService(ILocalStorageService localStorageService, INotifyService notifyService)
        {
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
            _notifyService = notifyService ?? throw new ArgumentNullException(nameof(notifyService));
        }

        public async Task<IEnumerable<AirFortuneTable>> GetTables()
        {
            return await _localStorageService.GetItemAsync<List<AirFortuneTable>>(TablesStorageKey) 
                   ?? new List<AirFortuneTable>();
        }

        public void ChangeTable(AirFortuneTable table)
        {
            _selectedTable = table;
            _notifyService.SelectTable(table);
        }
        
    }
}
