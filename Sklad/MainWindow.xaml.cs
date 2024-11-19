using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            SELECT i.id, i.name, i.catalog_number, i.type, s.name AS subtype, i.quantity, i.location
            FROM Items i
            LEFT JOIN Subtypes s ON i.subtype_id = s.id";

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
                            Subtype = reader.IsDBNull(4) ? null : reader.GetString(4),
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
    }
}