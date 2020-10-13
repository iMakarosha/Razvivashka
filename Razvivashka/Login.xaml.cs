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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Razvivashka.Xml;

namespace Razvivashka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window
    {
        public string userName;

        public Login()
        {
            InitializeComponent();
            List<string> userNames = XmlFunctions.XmlReader();
            foreach(string Name in userNames)
            {
                cbUsers.Items.Add(Name);
            }
            cbUsers.Items.Add("Меня нет в списке!");
        }

        private void CbUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbUsers.SelectedIndex == cbUsers.Items.Count - 1)
            {
                LoginWindow.Height += 100;
                regPanel.Visibility = Visibility.Visible;
            }
            else
            {
                userName = cbUsers.SelectedItem.ToString();
                this.DialogResult = true;
            }
        }
        
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = tbUserName.Text.Split(' ');
            if (user.Count() == 2)
            {
                user[0] = Char.ToUpper(user[0][0]) + user[0].Substring(1).ToLower();
                user[1] = Char.ToUpper(user[1][0]) + user[1].Substring(1).ToLower();
                string userN = user[0] + " " + user[1];
                XmlFunctions.AddNewUser(userN);
                userName = userN;
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Нужно ввести имя и фамилию!");
            }
        }
    }
}
