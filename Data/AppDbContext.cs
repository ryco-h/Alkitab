using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Alkitab.Models;
using Dapper;
using Npgsql;

// For Task<>

public class DatabaseManager
{
    private readonly string connectionString =
        "Host=localhost;Port=5433;Database=alkitab;Username=postgres;Password=noctynal";

    private IDbConnection Connection => new NpgsqlConnection(connectionString);

    public async Task<IEnumerable<Kitab>> GetBible()
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());
            Console.WriteLine("Connected to " + connectionString);
            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name as BookName, * FROM kitab"
            );

            return result;
        }
    }

    public async Task<IEnumerable<BibleInstances>> GetBookList()
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());
            var result = await connection.QueryAsync<BibleInstances>(
                "SELECT book_id, book_name as BookName FROM daftar_kitab"
            );

            return result;
        }
    }

    public async Task<IEnumerable<Kitab>> GetBibleByBook(string bookName)
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());
            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name as BookName, * FROM kitab WHERE book_name='" + bookName + "'"
            );

            Console.WriteLine("Result: " + result);
            return result;
        }
    }

    public async Task<IEnumerable<Kitab>> FilterBible(string bookName, string chapter)
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());
            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name as BookName, * FROM kitab WHERE book_name='" + bookName + "' AND chapter='" +
                chapter + "'"
            );

            return result;
        }
    }
}