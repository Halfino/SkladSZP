using Sklad.Data;
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
        public ItemFormWindow(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
        }

        public User LoggedInUser { get; private set; }
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
            if (string.IsNullOrWhiteSpace(CommentTextBox.Text))  // Zkontrolujeme, jestli je Comment prázdné
            {
                MessageBox.Show("Komentář nesmí být prázdný.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Zastavíme další provádění metody, pokud je komentář prázdný
            }

            if (CurrentItem == null)
                CurrentItem = new Item();

            // Aktualizace dat z formuláře
            CurrentItem.Name = NameTextBox.Text;
            CurrentItem.CatalogNumber = CatalogNumberTextBox.Text;
            CurrentItem.Type = TypeComboBox.SelectedItem?.ToString(); // Ujistíme se, že získáme hodnotu typu jako string
            CurrentItem.Subtype = SubtypeComboBox.SelectedItem?.ToString();
            CurrentItem.Quantity = int.TryParse(QuantityTextBox.Text, out int quantity) ? quantity : 0;
            CurrentItem.Location = LocationTextBox.Text;
            CurrentItem.Comment = CommentTextBox.Text;
            CurrentItem.Modified_by = LoggedInUser.Name;

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

    }
}