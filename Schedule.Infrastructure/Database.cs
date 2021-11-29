using Dapper;
using Microsoft.Extensions.Configuration;
using Schedule.Domain.Interfaces.Data;
using Serilog;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Infrastructure
{
    public class Database : IDatabase
    {
        private readonly IConfiguration _configuration;

        public Database(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DbConnection> CreateAndOpenConnection(CancellationToken stoppingToken = default)
        {
            var connection = new SqlConnection(GetConnectionString());
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            await connection.OpenAsync(stoppingToken);

            return connection;
        }

        public async Task ExecuteInTransaction(Func<DbConnection, DbTransaction, Task> action, CancellationToken cancellationToken = default)
        {
            await using var conn = await CreateAndOpenConnection(cancellationToken);
            await using var transaction = await conn.BeginTransactionAsync(cancellationToken);
            try
            {
                await action(conn, transaction);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                Log.Error(ex, "Task canceled transaction - rolling back");
                await transaction.RollbackAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception while executing transaction - rolling back");
                try
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch (Exception ex2)
                {
                    Log.Error(ex2, "Error rolling back transaction");
                }

                throw;
            }
        }

        private string GetConnectionString()
        {
            return _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
        }

    }
}
