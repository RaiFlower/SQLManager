using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLManager.src;

public class Manager
{
    private Interface _interface = new();
    private ANSIFixer _ANSIFixer = new();

    private const char SavedQuerryPrefix = '?';

    public void Start()
    {
        string folderPath = Path.Combine(AppContext.BaseDirectory, "db");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string dbPath = Path.Combine(folderPath, "test.db");

        _interface = new();
        _interface.OpenConnection(dbPath);

        _interface.ExecuteSQL("CREATE TABLE IF NOT EXISTS hidden_SavedQuerries (Id INTEGER PRIMARY KEY, Name TEXT, QuerryString TEXT)");

        _ANSIFixer.EnableAnsiSupport();

        Console.Title = "SQL Manager";

        Update();
    }

    private void Update()
    {
        Console.Write("> ");
        string? command = Console.ReadLine();
        Console.WriteLine("");

        switch (command?.Split(' ')[0])
        {
            case "!Exit":
                End();
                return;

            case "!Help":
                Console.WriteLine("!Exit: Close application");
                Console.WriteLine("!Help: List all commands");
                Console.WriteLine("!Default: Build default table");
                Console.WriteLine("!Clear: Clear table by name");
                Console.WriteLine("!List: List all tables");
                Console.WriteLine("!Save: Save SQL querry under  thegiven name");
                Console.WriteLine("!Forget: Forget saved SQL querry under the given name");
                Console.WriteLine("!Saved: List all saved querries");
                break;
            
            case "!Default":
                _interface.ExecuteSQL("CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY, Name TEXT, Price REAL)");
                _interface.ClearTable("Items");
                _interface.ExecuteSQL("INSERT INTO Items (Name, Price) VALUES ('Keyboard', 49.99)");
                _interface.ExecuteSQL("INSERT INTO Items (Name, Price) VALUES ('Mouse', 19.99)");
                _interface.ExecuteSQL("INSERT INTO Items (Name, Price) VALUES ('Monitor', 199.99)");
                break;
            
            case "!Clear":
                _interface.ClearTable(command?.Split(' ')[1] ?? "");
                break;
            
            case "!List":
                _interface.GetAllTableNames().ForEach(name => Console.WriteLine(_interface.GetTableColumns(name)));
                break;
            
            case "!Save":
                _interface.ExecuteSQL($"INSERT INTO hidden_SavedQuerries (Name, QuerryString) VALUES ('{command?.Split(' ')[1]}', '{command?.Split(' ', 3)[2]}')");
                break;
            
            case "!Forget":
                _interface.ExecuteSQL($"DELETE FROM hidden_SavedQuerries WHERE Name = '{command?.Split(' ')[1]}'");
                break;
            
            case "!Saved":
                List<string> names = _interface.GetColumn<string>("hidden_SavedQuerries", "Name");
                List<string> querryStrings = _interface.GetColumn<string>("hidden_SavedQuerries", "QuerryString");
                for (int i = 0; i < names.Count; i++)
                {
                    Console.WriteLine(SavedQuerryPrefix + names[i] + ": " + querryStrings[i]);
                }
                break;
            
            case var cmd when cmd?.StartsWith('?') ?? false:
                _interface.ExecuteSQL((string)(_interface.ExecuteScalar($"SELECT QuerryString FROM hidden_SavedQuerries WHERE Name = '{command?[1..]}'") ?? ""));
                break;
            
            case "CREATE":
            case "INSERT":
            case "SELECT":
            case "DELETE":
            case "DROP":
                _interface.ExecuteSQL(command);
                break;

            default:
                Console.WriteLine("Ivalid command");
                break;
        }

        Console.WriteLine();
        Update();
    }

    private void End()
    {
        _interface.CloseConnection();

        Environment.Exit(0);
    }
}
