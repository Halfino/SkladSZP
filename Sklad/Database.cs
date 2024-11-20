using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Sklad.Models;

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
                        subtype TEXT NOT NULL,  -- Uložení subtype jako string
                        quantity INTEGER NOT NULL DEFAULT 0,
                        location TEXT
                    )";
                    using (var command = new SqliteCommand(createItemsTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // Vytvoreni tabulky ItemChanges
                    var createItemChangesTableCmd = @"
                    CREATE TABLE ItemChanges (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ItemId INTEGER NOT NULL,
                        ChangeType TEXT NOT NULL,
                        ChangeDate TEXT NOT NULL,
                        Description TEXT,
                        FOREIGN KEY (ItemId) REFERENCES Items (Id)
                    );";
                    using (var command = new SqliteCommand(createItemChangesTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void AddItemToDatabase(Item item)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = new SqliteCommand("INSERT INTO Items (Name, catalog_number, Type, Subtype, Quantity, Location) VALUES (@Name, @catalog_number, @Type, @Subtype, @Quantity, @Location)", connection);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@catalog_number", item.CatalogNumber);
                command.Parameters.AddWithValue("@Type", item.Type);
                command.Parameters.AddWithValue("@Subtype", item.Subtype);
                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                command.Parameters.AddWithValue("@Location", item.Location);
                command.ExecuteNonQuery();

                // Získání ID právě přidané položky
                var lastIdCommand = new SqliteCommand("SELECT last_insert_rowid()", connection);
                long lastId = (long)lastIdCommand.ExecuteScalar();

                // Zapsání změny do historie
                var historyCommand = new SqliteCommand("INSERT INTO ItemChanges (ItemId, ChangeType, ChangeDate, Description) VALUES (@ItemId, @ChangeType, @ChangeDate, @Description)", connection);
                historyCommand.Parameters.AddWithValue("@ItemId", lastId);
                historyCommand.Parameters.AddWithValue("@ChangeType", "Create");
                historyCommand.Parameters.AddWithValue("@ChangeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                historyCommand.Parameters.AddWithValue("@Description", $"Položka {item.Name} přidána.");
                historyCommand.ExecuteNonQuery();
            }
        }

        public static void UpdateItemInDatabase(Item item)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = new SqliteCommand("UPDATE Items SET Name = @Name, catalog_number = @catalog_number, Quantity = @Quantity, Location = @Location WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@catalog_number", item.CatalogNumber);
                command.Parameters.AddWithValue("@Quantity", item.Quantity);
                command.Parameters.AddWithValue("@Location", item.Location);
                command.Parameters.AddWithValue("@Id", item.Id);
                command.ExecuteNonQuery();

                // Zapsání změny do historie
                var historyCommand = new SqliteCommand("INSERT INTO ItemChanges (ItemId, ChangeType, ChangeDate, Description) VALUES (@ItemId, @ChangeType, @ChangeDate, @Description)", connection);
                historyCommand.Parameters.AddWithValue("@ItemId", item.Id);
                historyCommand.Parameters.AddWithValue("@ChangeType", "Update");
                historyCommand.Parameters.AddWithValue("@ChangeDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                historyCommand.Parameters.AddWithValue("@Description", $"Položka {item.Name} upravena.");
                historyCommand.ExecuteNonQuery();
            }
        }
    }
}
