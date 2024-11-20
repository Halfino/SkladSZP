using Sklad.Data;
using Sklad.Models;
using System.Windows;


namespace Sklad
{
    /// <summary>
    /// Interakční logika pro NewItemForm.xaml
    /// </summary>
    public partial class NewItemForm : Window
    {
        public NewItemForm()
        {
            InitializeComponent();
        }

        public Item CurrentItem { get; set; } // Pro úpravu záznamu

        // Metoda pro dynamické načítání podtypů
        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SubtypeComboBox.Items.Clear();

            if (TypeComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selected)
            {
                string type = selected.Content.ToString();
                LoadSubtypes(type);
            }
        }

        // Načítání podtypů podle vybraného typu
        private void LoadSubtypes(string type)
        {
            // Načteme podtypy na základě typu
            SubtypeComboBox.Items.Clear(); // Nezapomeňme vyprázdnit předchozí nabídku
            List<Subtype> subtypes = new List<Subtype>();
            if (type == "ND")
            {
                subtypes = Database.GetNdSubtypes();
            }
            else if (type == "Material")
            {
                subtypes = Database.GetMatSubtypes();
            }

            foreach (var subtype in subtypes)
            {
                SubtypeComboBox.Items.Add(subtype.Name);
            }
        }

        // Uložení nového záznamu
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Logika pro uložení nového záznamu, zde si můžete upravit podle potřeby
            if (CurrentItem == null)
                CurrentItem = new Item();
            {
                CurrentItem.Name = NameTextBox.Text;
                CurrentItem.CatalogNumber = CatalogNumberTextBox.Text;
                CurrentItem.Type = ((System.Windows.Controls.ComboBoxItem)TypeComboBox.SelectedItem)?.Content.ToString();
                CurrentItem.Subtype = SubtypeComboBox.SelectedItem?.ToString();
                CurrentItem.Quantity = int.TryParse(QuantityTextBox.Text, out int quantity) ? quantity : 0;
                CurrentItem.Location = LocationTextBox.Text;
            };

            // Přidání nového záznamu do kolekce nebo databáze
            // Tady zavoláte metodu pro uložení dat

            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
