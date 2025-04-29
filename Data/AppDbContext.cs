using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Alkitab.Models;
using Dapper;
using Microsoft.Data.Sqlite;

public class DatabaseManager
{
    // private readonly string connectionString =
    //     "Host=localhost;Port=5433;Database=alkitab;Username=postgres;Password=noctynal";
    // private IDbConnection Connection => new NpgsqlConnection(connectionString);

    private readonly string _connectionString = "Data Source=C:\\Dudu\\Programming\\Projects\\Exercise\\DotNet\\Alkitab\\alkitab.db";

    private SqliteConnection Connection => new(_connectionString);


    /// <summary>
    ///     Bible DB Interactions
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Kitab>> GetBible()
    {
        using (var connection = Connection)
        {
            await Task.Run(() => connection.Open());

            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name as BookName, * FROM kitab"
            );

            return result;
        }
    }

    public async Task<IEnumerable<BibleInstances>> GetBookList()
    {
        using var connection = Connection;
        await connection.OpenAsync();

        try
        {
            var result = await connection.QueryAsync<BibleInstances>(
                "SELECT book_id, book_name AS BookName FROM daftar_kitab"
            );
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dapper Query Error: " + ex.Message);
            return Enumerable.Empty<BibleInstances>();
        }
    }

    public async Task<IEnumerable<Kitab>> GetBibleByBook(string bookName)
    {
        using var connection = Connection;
        await connection.OpenAsync();

        try
        {
            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name AS BookName, * FROM kitab WHERE book_name = @BookName",
                new { BookName = bookName }
            );
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dapper Query Error: " + ex.Message);
            return Enumerable.Empty<Kitab>();
        }
    }

    public async Task FilterBible(string bookName, string chapter, ObservableCollection<Kitab> targetCollection)
    {
        using var connection = Connection;
        await connection.OpenAsync();

        try
        {
            var result = await connection.QueryAsync<Kitab>(
                "SELECT book_name AS BookName, * FROM kitab " +
                "WHERE book_name = @BookName AND chapter = @Chapter " +
                "ORDER BY CAST(verse AS INTEGER)",
                new { BookName = bookName, Chapter = int.Parse(chapter) }
            );

            // Update collection on UI thread if needed
            targetCollection.Clear();
            foreach (var item in result)
                targetCollection.Add(item);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dapper Query Error: " + ex.Message);
            targetCollection.Clear();
        }
    }

    /// <summary>
    ///     Bookmark DB Interactions
    /// </summary>
    /// <returns></returns>
    public async Task InsertBookmark(
        string bookName,
        string chapter,
        string verse
    )
    {
        using var connection = Connection;
        await connection.OpenAsync();

        try
        {
            var insertSql = @"
                INSERT INTO bookmark (BookName, Chapter, Verse) 
                VALUES (@BookName, @Chapter, @Verse);
                SELECT last_insert_rowid();";

            var result = await connection.ExecuteScalarAsync<long>(
                insertSql,
                new { BookName = bookName, Chapter = chapter, Verse = verse }
            );

            if (result > 0)
            {
                Console.WriteLine($"Insert successful! New ID: {result}");
            }
            else
            {
                Console.WriteLine("Insert failed.");
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dapper Query Error: " + ex.Message);
        }
    }
    
    public async Task GetAllBookmarks(ObservableCollection<Bookmark> targetCollection)
    {
        using var connection = Connection;
        await connection.OpenAsync();

        try
        {
            var selectSql = @"SELECT 
            bookname as Bookname,
            chapter as Chapter,
            verse as Verse,
            *
            FROM bookmark;";

            var result = await connection.QueryAsync<Bookmark>(
                selectSql
            );
            
            targetCollection.Clear();
            foreach (var item in result)
            {
                targetCollection.Add(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dapper Query Error: " + ex.Message);
        }
    }
    
    public async Task RemoveBookmark(string id, ObservableCollection<Bookmark> targetCollection)
    {
        using var connection = Connection;
        await connection.OpenAsync();

        try
        {
            var sql = "DELETE FROM bookmark WHERE id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = int.Parse(id) });
            
            var item = targetCollection.FirstOrDefault(b => b.id == id);
            if (item != null)
            {
                targetCollection.Remove(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dapper Query Error: " + ex.Message);
        }
    }
}