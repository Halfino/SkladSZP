using Sklad.Models;
using System.Windows;
using System.Windows.Controls;

namespace Sklad
{
    /// <summary>
    /// Interakční logika pro ItemFormWindow.xaml
    /// </summary>
    public partial class ItemFormWindow : Window
    {
        public ItemFormWindow()
        {
            InitializeComponent();
        }

        public Item CurrentItem { get; set; } // Pro úpravu záznamu

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SubtypeComboBox.Items.Clear();

            // Ujistíme se, že vybraná položka je string
            if (TypeComboBox.SelectedItem is string type)
            {
                LoadSubtypes(type);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem == null)
                CurrentItem = new Item();

            // Aktualizace dat z formuláře
            CurrentItem.Name = NameTextBox.Text;
            CurrentItem.CatalogNumber = CatalogNumberTextBox.Text;
            CurrentItem.Type = TypeComboBox.SelectedItem?.ToString(); // Ujistíme se, že získáme hodnotu typu jako string
            CurrentItem.Subtype = SubtypeComboBox.SelectedItem?.ToString();
            CurrentItem.Quantity = int.TryParse(QuantityTextBox.Text, out int quantity) ? quantity : 0;
            CurrentItem.Location = LocationTextBox.Text;

            // Logika pro přidání/úpravu položky zde
            // Např. zavolání metody UpdateItem nebo InsertItem

            DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void SetItemForEdit(Item item)
        {
            CurrentItem = item;

            // Přiřadíme hodnoty z CurrentItem do kontrolních prvků (TextBoxů, ComboBoxů)
            NameTextBox.Text = item.Name;
            CatalogNumberTextBox.Text = item.CatalogNumber;
            QuantityTextBox.Text = item.Quantity.ToString();
            LocationTextBox.Text = item.Location;

            // Nastavení ComboBoxu pro typ (ND nebo Material)
            TypeComboBox.Items.Clear();  // Ujistíme se, že nebudou duplikáty v ComboBoxu
            TypeComboBox.Items.Add("ND");
            TypeComboBox.Items.Add("Material");

            // Nastavíme vybraný typ
            TypeComboBox.SelectedItem = item.Type;
            LoadSubtypes(item.Type);

            // Nastavíme vybraný podtyp podle hodnoty Subtype (string)
            if (!string.IsNullOrEmpty(item.Subtype))
            {
                SubtypeComboBox.SelectedItem = item.Subtype;
            }

            // Zakážeme možnost změny typu a podtypu při úpravách
            if (item.Id > 0) // Pokud je ID větší než 0, znamená to, že jde o úpravu existující položky
            {
                TypeComboBox.IsEnabled = false;
                SubtypeComboBox.IsEnabled = false;
            }
            else
            {
                TypeComboBox.IsEnabled = true;
                SubtypeComboBox.IsEnabled = true;
            }
        }

        private void LoadSubtypes(string type)
        {
            // Načteme podtypy na základě typu
            SubtypeComboBox.Items.Clear(); // Nezapomeňme vyprázdnit předchozí nabídku

            if (type == "ND")
            {
                SubtypeComboBox.Items.Add("Trafo");
                SubtypeComboBox.Items.Add("Návěstidlo");
                SubtypeComboBox.Items.Add("Kabel");
                SubtypeComboBox.Items.Add("Deska zdroje");
            }
            else if (type == "Material")
            {
                SubtypeComboBox.Items.Add("Žárovka");
                SubtypeComboBox.Items.Add("Izolační materiál");
                SubtypeComboBox.Items.Add("Kabel");
                SubtypeComboBox.Items.Add("Koncovka");
            }

            // Zajistíme, že pokud je položka nová, bude k dispozici i nabídka podtypů
            if (SubtypeComboBox.Items.Count > 0)
            {
                SubtypeComboBox.SelectedIndex = 0; // Můžete nastavit výchozí hodnotu
            }
        }

    }
}