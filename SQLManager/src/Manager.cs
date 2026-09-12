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

    public void Start()
    {
        _interface = new();
        _interface.OpenConnection("db/test.db");

        _ANSIFixer.EnableAnsiSupport();

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
            
            case "CREATE":
            case "INSERT":
            case "SELECT":
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
