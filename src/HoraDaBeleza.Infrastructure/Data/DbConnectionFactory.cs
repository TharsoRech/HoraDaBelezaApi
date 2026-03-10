using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HoraDaBeleza.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
