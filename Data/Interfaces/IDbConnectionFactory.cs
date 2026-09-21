using Microsoft.Data.SqlClient;

namespace WebsiteQuanLyThuVien.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}