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
using Razvivashka.Xml;

namespace Razvivashka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UserInfo _userInfo;
        // string resourcesPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Resourses";
        string resourcesPath = XmlFunctions.path + "//Resourses";

        public MainWindow()
        {
            InitializeComponent();

            Login loginWindow = new Login();
            if (loginWindow.ShowDialog() == true)
            {
                _userInfo = XmlFunctions.XmlReader(loginWindow.userName);

                UserName.Content = "Привет, " + _userInfo.Name + "!";

                if (!(_userInfo.Lessons == null))
                {
                    foreach(var lesson in _userInfo.Lessons)
                    {
                        var curGrid = spMain.FindName("GrdLesson" + lesson._lessonId) as Grid;
                        int levelCounter = 0;

                        foreach (var levelTime in lesson._levelTime)
                        {
                            Image imgChecked = new Image();
                            imgChecked.Source = new BitmapImage(new Uri(resourcesPath + "//checked.png"));
                            imgChecked.Height = 20;
                            curGrid.Children.Add(imgChecked);
                            Grid.SetColumn(imgChecked, 1);
                            Grid.SetRow(imgChecked, levelCounter);

                            Label lbTime = new Label();
                            lbTime.Content = levelTime + " сек";
                            lbTime.FontSize = 14;
                            curGrid.Children.Add(lbTime);
                            Grid.SetColumn(lbTime, 2);
                            Grid.SetRow(lbTime, levelCounter++);
                        }

                        Expander expEl = spMain.FindName("ExpLesson" + lesson._lessonId) as Expander;
                    }
                }

            }
            else
            {
                MessageBox.Show("Приложение закрывается!");
                Application.Current.Shutdown();
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander curExpander = sender as Expander;
            foreach(var expander in FindVisualChildren<Expander>(spMain))
            {
                if (expander.Header != curExpander.Header)
                {
                    expander.IsExpanded = false;
                }
            }
            
        }

        private void StartLesson_Click(object sender, RoutedEventArgs e)
        {
            Button curButton = sender as Button;
            Expander expEl = spMain.FindName("Exp" + curButton.Name.Substring(3)) as Expander;
            int counter = 0;
            foreach(RadioButton radioButton in FindVisualChildren<RadioButton>(expEl))
            {
                if (radioButton.IsChecked != true)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }
            
            Lesson lesson = new Lesson(_userInfo, expEl.Name, counter <= 3 ? (++counter) : 0);
            lesson.Owner = this;
            this.Hide();
            lesson.ShowDialog();
            //if (lesson.DialogResult == true)
            //{
            //    MessageBox.Show("2");
            //}
            //обновить грид с пройденными уровнями для текущего exp
            this.Show();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
