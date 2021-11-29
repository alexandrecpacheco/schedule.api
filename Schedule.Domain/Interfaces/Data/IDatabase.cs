using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data
{
    public interface IDatabase
    {
        Task<DbConnection> CreateAndOpenConnection(CancellationToken stoppingToken = default);
        Task ExecuteInTransaction(Func<DbConnection, DbTransaction, Task> action,
            CancellationToken cancellationToken = default);
        void UpgradeIfNecessary();
    }
}
