using Microsoft.Data.SqlClient;

namespace WebsiteQuanLyThuVien.Data;


public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "DefaultConnection is not configured.");
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

    SqlConnection IDbConnectionFactory.CreateConnection()
    {
        throw new NotImplementedException();
    }
}