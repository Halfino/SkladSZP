using Microsoft.Data.Sqlite;
using Sklad.Data;
using Sklad.Models;
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
    /// Interakční logika pro ItemHistory.xaml
    /// </summary>
    public partial class ItemHistory : Window
    {
        public int SelectedItemId { get; set; } // ID vybrané položky

        public ItemHistory(int itemId, Item item)
        {
            InitializeComponent();
            this.Title = "Historie Položky " + item.Name;
            SelectedItemId = itemId;
            LoadItemHistory();
        }

        private void LoadItemHistory()
        {
            ItemHistoryDataGrid.ItemsSource = Database.GetItemHistory(SelectedItemId);
        }
    }
}
