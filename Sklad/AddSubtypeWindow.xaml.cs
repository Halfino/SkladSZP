using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sklad
{
    /// <summary>
    /// Interakční logika pro AddSubtypeWindow.xaml
    /// </summary>
    public partial class AddSubtypeWindow : Window
    {
        public string SubtypeName { get; private set; }
        public string SelectedType { get; private set; }

        public AddSubtypeWindow()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SubtypeNameTextBox.Text))
            {
                MessageBox.Show("Název podtypu nemůže být prázdný!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (TypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SubtypeName = SubtypeNameTextBox.Text.Trim();
                SelectedType = selectedItem.Tag.ToString();
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Musíte vybrat typ položky!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
