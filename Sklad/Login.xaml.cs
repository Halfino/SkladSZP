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
    /// Interakční logika pro Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        public User LoggedInUser { get; private set; }

        List<User> users = new List<User>
            {
                new User { Name = "admin", UserName = "admin", Password = "admin123" },
                new User { Name = "Jan Koranda", UserName = "KorandaJ", Password = "KorandaJ" },
                new User { Name = "Karel Mahelka",UserName = "MahelkaK", Password = "MahelkaK" },
                new User { Name = "František Franc",UserName = "FrancF", Password = "FrancF" },
                new User { Name = "Jiří Pošíval",UserName = "PosivalJ", Password = "PosivalJ" },
                new User { Name = "Petr Eliáš",UserName = "EliasP", Password = "EliasP" },
                new User { Name = "Miloslav Janota",UserName = "JanotaM", Password = "JanotaM" }
            };

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string enteredUser = UserNameTextBox.Text;
            string enteredPassword = PasswordBox.Password;

            var user = users.FirstOrDefault(u => u.UserName == enteredUser && u.Password == enteredPassword);
            if (user != null)
            {
                LoggedInUser = user;

                MessageBox.Show("Přihlášení proběhlo úspěšně.");
                MainWindow mainWindow = new MainWindow(LoggedInUser);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Chybné přihlašovací údaje.");
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e); // Vyvolání kliknutí na tlačítko při stisknutí Enter
            }
        }
    }
}
