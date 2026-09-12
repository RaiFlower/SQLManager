using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace SQLManager.src;

public class Interface
{
    private SqliteConnection _connection = new();

    public void OpenConnection(string connectionString)
    {
        _connection = new($"Data Source={connectionString}");
        _connection.Open();
    }

    public void CloseConnection()
    {
        _connection.Close();
    }

    public void ExecuteSQL(string sql)
    {
        switch (sql?.Split(' ')[0])
        {
            case "CREATE":
            case "INSERT":
                ExecuteNonQuerry(sql);
                break;
            
            case "SELECT":
                ExecuteQuerry(sql);
                break;
        }
    }

    private void ExecuteNonQuerry(string sql)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private void ExecuteQuerry(string sql)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;
        var reader = command.ExecuteReader();
        PrintTable(reader);
    }

    private void PrintTable(DbDataReader reader)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            Console.Write("\u001b[4m" + reader.GetName(i).PadRight(15) + "\u001b[0m");
        }
        Console.WriteLine();

        while (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                object value = reader.GetValue(i);
                Console.Write((value.ToString() ?? "null").PadRight(15));
            }
            Console.WriteLine();
        }
    }

    public void ClearTable(string tableName)
    {
        if (!TableExists(tableName))
            return;
        
        var command = _connection.CreateCommand();
        command.CommandText = $"DELETE FROM {tableName}";
        command.ExecuteNonQuery();
    }

    private bool TableExists(string tableName)
    {
        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";

        using var reader = command.ExecuteReader();
        return reader.HasRows;
    }

    public List<string> GetAllTableNames()
    {
        var tables = new List<string>();

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }

    public string GetTableColumns(string tableName)
    {
        var columnNames = new List<string>();

        var command = _connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName})";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            columnNames.Add(reader.GetString(1));
        }

        return $"{tableName} ({string.Join(", ", columnNames)})";
    }
}
