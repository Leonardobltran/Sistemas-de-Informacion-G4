using Microsoft.Data.SqlClient;

namespace HuixinMotors.DAL.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string ConnectionString => _connectionString;

    public SqlConnection Create()
    {
        return new SqlConnection(_connectionString);
    }
}
