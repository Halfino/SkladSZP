using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Sklad.Data
{
    public class Database
    {
        private const string DatabaseFileName = "warehouse.db";

        public static string DatabasePath => Path.Combine(Directory.GetCurrentDirectory(), DatabaseFileName);

        public static void Initialize()
        {
            if (!File.Exists(DatabasePath))
            {
                using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
                {
                    connection.Open();

                    // Vytvoření tabulky Items
                    var createItemsTableCmd = @"
                    CREATE TABLE Items (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        catalog_number TEXT NOT NULL UNIQUE,
                        type TEXT NOT NULL CHECK(type IN ('ND', 'Material')),
                        subtype_id INTEGER,
                        quantity INTEGER NOT NULL DEFAULT 0,
                        location TEXT,
                        FOREIGN KEY (subtype_id) REFERENCES Subtypes(id)
                    )";
                    using (var command = new SqliteCommand(createItemsTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Vytvoření tabulky Subtypes
                    var createSubtypesTableCmd = @"
                    CREATE TABLE Subtypes (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        type TEXT NOT NULL CHECK(type IN ('ND', 'Material')),
                        name TEXT NOT NULL
                    )";
                    using (var command = new SqliteCommand(createSubtypesTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Přidání výchozích podtypů do tabulky Subtypes
                    var insertSubtypesCmd = @"
                    INSERT INTO Subtypes (type, name) VALUES
                    ('ND', 'Trafo'),
                    ('ND', 'Návěstidlo'),
                    ('ND', 'Kabel'),
                    ('ND', 'Deska zdroje'),
                    ('Material', 'Žárovka'),
                    ('Material', 'Izolační materiál'),
                    ('Material', 'Kabel'),
                    ('Material', 'Koncovka')";
                    using (var command = new SqliteCommand(insertSubtypesCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
