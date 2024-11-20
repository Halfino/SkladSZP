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
                        catalog_number TEXT NOT NULL,
                        type TEXT NOT NULL CHECK(type IN ('ND', 'Material')),
                        subtype TEXT NOT NULL,  -- Uložení subtype jako string
                        quantity INTEGER NOT NULL DEFAULT 0,
                        location TEXT, 
	                    last_modified_by TEXT,
	                    last_modified_at TIMESTAMP
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

                    //Vytvoření tabulky ItemHistory
                    var createItemHistoryTableCmd = @"
                        CREATE TABLE ItemHistory (
                            id INTEGER PRIMARY KEY AUTOINCREMENT,
                            item_id INTEGER NOT NULL, 
                            name TEXT, 
                            catalog_number TEXT, 
                            type TEXT, 
                            subtype TEXT, 
                            quantity INTEGER, 
                            location TEXT,
                            comment TEXT, 
                            modified_by TEXT NOT NULL,
                            modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP, 
                            FOREIGN KEY (item_id) REFERENCES Items (id) ON DELETE CASCADE
                        );";
                    using(var command = new SqliteCommand(createItemHistoryTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    //Vytvoření tabulky Subtypes u materiálu
                    var createMatSubtypesTableCmd = @"
                        CREATE TABLE mat_subtypes (
                            name TEXT NOT NULL
                        )";
                    using (var command = new SqliteCommand(createMatSubtypesTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    //Vytvoření tabulky Subtypes u Náhradních dílů
                    var createNdSubtypesTableCmd = @"
                        CREATE TABLE nd_subtypes (
                            name TEXT NOT NULL
                        )";
                    using ( var command = new SqliteCommand(createNdSubtypesTableCmd, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Database.seedDB();
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

        public static List<Subtype> GetNdSubtypes()
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT name FROM nd_subtypes", connection);
                var reader = command.ExecuteReader();

                var subtypes = new List<Subtype>();
                while (reader.Read())
                {
                    subtypes.Add(new Subtype { Name = reader.GetString(0) });
                }

                return subtypes;
            }
        }

        public static List<Subtype> GetMatSubtypes()
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();
                var command = new SqliteCommand("SELECT name FROM mat_subtypes", connection);
                var reader = command.ExecuteReader();

                var subtypes = new List<Subtype>();
                while (reader.Read())
                {
                    subtypes.Add(new Subtype { Name = reader.GetString(0) });
                }

                return subtypes;
            }
        }

        public static void SaveItemHistory(Item item)
        {
            using (var connection = new SqliteConnection($"Data Source={Database.DatabasePath}"))
            {
                connection.Open();
                var command = new SqliteCommand("INSERT INTO ItemHistory (item_id, name, catalog_number, type, subtype, quantity, location, comment, modified_by) " +
                                                "VALUES (@item_id, @name, @catalog_number, @type, @subtype, @quantity, @location, @comment, @modified_by)", connection);
                command.Parameters.AddWithValue("@item_id", item.Id);
                command.Parameters.AddWithValue("@name", item.Name);
                command.Parameters.AddWithValue("@catalog_number", item.CatalogNumber);
                command.Parameters.AddWithValue("@type", item.Type);
                command.Parameters.AddWithValue("@subtype", item.Subtype);
                command.Parameters.AddWithValue("@quantity", item.Quantity);
                command.Parameters.AddWithValue("@location", item.Location);
                command.Parameters.AddWithValue("@comment", item.Comment);
                command.Parameters.AddWithValue("@modified_by", item.Modified_by);

                command.ExecuteNonQuery();
            }
        }

        public static List<Item> GetItemHistory(int SelectedItemId)
        {
            using (var connection = new SqliteConnection($"Data Source={DatabasePath}"))
            {
                connection.Open();

                string query = "SELECT * FROM ItemHistory WHERE item_id = @itemId";
                var command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@itemId", SelectedItemId);

                var reader = command.ExecuteReader();

                var historyList = new List<Item>();

                while (reader.Read())
                {
                    var historyItem = new Item
                    {
                        Id = reader.GetInt32(1), // Adjust based on column index
                        Name = reader.GetString(2),
                        CatalogNumber = reader.GetString(3),
                        Type = reader.GetString(4),
                        Subtype = reader.GetString(5),
                        Quantity = reader.GetInt32(6),
                        Location = reader.GetString(7),
                        Comment = reader.GetString(8),
                        Modified_by = reader.GetString(9),
                        Modified_at = reader.GetDateTime(10).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    historyList.Add(historyItem);
                }

                return historyList;
            }
        }

        private static void seedDB()
        {
            List<String> ndTypes = new List<String>();
            List<String> matTypes = new List<String>();
            matTypes.Add("Žárovka");
            matTypes.Add("Izolační materiál");
            matTypes.Add("Kabel");
            matTypes.Add("Koncovka");

            ndTypes.Add("Trafo");
            ndTypes.Add("Návěstidlo");
            ndTypes.Add("ND Zdroje");

            using (var connection = new SqliteConnection($"Data Source={Database.DatabasePath}"))
            {
                foreach (String type in matTypes)
                {
                    connection.Open();
                    var command = new SqliteCommand("INSERT INTO mat_subtypes(name) " +
                                                    "VALUES (@name)", connection);
                    command.Parameters.AddWithValue("@name", type);

                    command.ExecuteNonQuery();
                }

                foreach (String type in ndTypes)
                {
                    connection.Open();
                    var command = new SqliteCommand("INSERT INTO nd_subtypes(name) " +
                                                    "VALUES (@name)", connection);
                    command.Parameters.AddWithValue("@name", type);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
