using System.Data;
using System.Windows;
using Microsoft.Data.Sqlite;
using Sklad.Data;
using Sklad.Models;


namespace Sklad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadItems();
        }

        private void LoadItems()
        {
            var items = new List<Item>();

            using (var connection = new SqliteConnection($"Data Source={Database.DatabasePath}"))
            {
                connection.Open();
                string query = @"
            SELECT id, name, catalog_number, type, subtype, quantity, location
            FROM Items";

                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Item
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CatalogNumber = reader.GetString(2),
                            Type = reader.GetString(3),
                            Subtype = reader.GetString(4), // Načítání Subtype jako string přímo z tabulky Items
                            Quantity = reader.GetInt32(5),
                            Location = reader.GetString(6)
                        });
                    }
                }
            }

            // Filtrování a přiřazení dat tabulkám
            SparePartsGrid.ItemsSource = items.Where(i => i.Type == "ND").ToList();
            ConsumablesGrid.ItemsSource = items.Where(i => i.Type == "Material").ToList();
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            NewItemForm form = new NewItemForm();
            if (form.ShowDialog() == true)
            {
                // Uložení nové položky do databáze
                Database.AddItemToDatabase(form.CurrentItem);
                RefreshDataGrid(); // Aktualizace zobrazené tabulky
            }
        }

        private void EditItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (SparePartsGrid.SelectedItem is Item selectedItem)
            {
                ItemFormWindow form = new ItemFormWindow();
                form.CurrentItem = selectedItem;
                form.SetItemForEdit(selectedItem);
                if (form.ShowDialog() == true)
                {
                    // Aktualizace položky v databázi
                    Database.UpdateItemInDatabase(form.CurrentItem);
                    RefreshDataGrid(); // Aktualizace zobrazené tabulky
                }
            }
            else if (ConsumablesGrid.SelectedItem is Item selectedConsumableItem)
            {
                ItemFormWindow form = new ItemFormWindow();
                form.CurrentItem = selectedConsumableItem;
                form.SetItemForEdit(selectedConsumableItem);
                if (form.ShowDialog() == true)
                {
                    // Aktualizace položky v databázi
                    Database.UpdateItemInDatabase(form.CurrentItem);
                    RefreshDataGrid(); // Aktualizace zobrazené tabulky
                }
            }
            else
            {
                MessageBox.Show("Vyberte položku k úpravě.", "Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RefreshDataGrid()
        {
            using (var connection = new SqliteConnection($"Data Source={Database.DatabasePath}"))
            {
                connection.Open();

                // Načtení náhradních dílů
                var sparePartsCommand = new SqliteCommand("SELECT * FROM Items WHERE Type = 'ND'", connection);
                var sparePartsReader = sparePartsCommand.ExecuteReader();
                var sparePartsList = new List<Item>();
                while (sparePartsReader.Read())
                {
                    sparePartsList.Add(new Item
                    {
                        Id = sparePartsReader.GetInt32(0),
                        Name = sparePartsReader.GetString(1),
                        CatalogNumber = sparePartsReader.GetString(2),
                        Type = sparePartsReader.GetString(3),
                        Subtype = sparePartsReader.GetString(4),
                        Quantity = sparePartsReader.GetInt32(5),
                        Location = sparePartsReader.GetString(6),
                    });
                }
                SparePartsGrid.ItemsSource = sparePartsList;

                // Načtení spotřebního materiálu
                var consumablesCommand = new SqliteCommand("SELECT * FROM Items WHERE Type = 'Material'", connection);
                var consumablesReader = consumablesCommand.ExecuteReader();
                var consumablesList = new List<Item>();
                while (consumablesReader.Read())
                {
                    consumablesList.Add(new Item
                    {
                        Id = consumablesReader.GetInt32(0),
                        Name = consumablesReader.GetString(1),
                        CatalogNumber = consumablesReader.GetString(2),
                        Type = consumablesReader.GetString(3),
                        Subtype = consumablesReader.GetString(4),
                        Quantity = consumablesReader.GetInt32(5),
                        Location = consumablesReader.GetString(6),
                    });
                }
                ConsumablesGrid.ItemsSource = consumablesList;
            }
        }
    }
}