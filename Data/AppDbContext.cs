using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks; // For Task<>
using Dapper;
using Npgsql;

public class DatabaseManager
{
    private string connectionString =
        "Host=localhost;Port=5433;Database=alkitab;Username=postgres;Password=noctynal";

    public IDbConnection Connection => new NpgsqlConnection(connectionString);

    public async Task<IEnumerable<Kitab>> GetBible()
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());
            System.Console.WriteLine("Connected to " + connectionString);
            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name as BookName, * FROM kitab"
            );

            return result;
        }
    }

    public async Task<IEnumerable<DaftarKitab>> GetBookList()
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());
            var result = await connection.QueryAsync<DaftarKitab>(
                "SELECT book_id, book_name as BookName FROM daftar_kitab"
            );

            return result;
        }
    }
}
