﻿using System;
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
                _userInfo = UpdateUserInfo(loginWindow.userName);

                UserName.Content = "Привет, " + _userInfo.Name + "!";
            }
            else
            {
                Application.Current.Shutdown();
            }

            List<StatisticLesson> statisticLessons = XmlFunctions.XmlReader(true);
            foreach(StatisticLesson lesson in statisticLessons)
            {
                foreach (var userStatistick in lesson._usersInfo)
                {
                    XmlFunctions.FindVisualChildren<Label>(GrdStatistick, lesson._lessonId - 1).Content += userStatistick._name + "\r\n";
                    XmlFunctions.FindVisualChildren<Label>(GrdStatistick, lesson._lessonId + 2).Content += userStatistick._commonTime.ToString().Substring(3) + " сек.\r\n";
                    //lbLess2.Content += userStatistick._commonTime + "\r\n";

                }
            }

            tbctrlLessons.Height = this.Height - 140;
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander curExpander = sender as Expander;
            foreach(var expander in XmlFunctions.FindVisualChildren<Expander>(spMain))
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
            foreach(RadioButton radioButton in XmlFunctions.FindVisualChildren<RadioButton>(expEl))
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

            LessonWindow lessonWind = new LessonWindow(_userInfo, expEl.Name, counter <= 3 ? (++counter) : 0);
            lessonWind.Owner = this;
            this.Hide();
            lessonWind.ShowDialog();
            _userInfo = UpdateUserInfo(_userInfo.Name);
            this.Show();
        }

        private UserInfo UpdateUserInfo(string userName)
        {
            _userInfo = XmlFunctions.XmlReader(userName);

            for(int i = 1; i <= 3; i++)
            {
                Grid curGrid = spMain.FindName("GrdLesson" + i) as Grid;
                int levelCounter = 0;
                ClearGridCels(curGrid);

                if (!(_userInfo.Lessons == null))
                {
                    foreach (Lesson lesson in _userInfo.Lessons)
                    {
                        if (lesson._lessonId == i)
                        {
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

                                curGrid.Children[levelCounter].IsEnabled = true;
                            }

                            Expander expEl = spMain.FindName("ExpLesson" + lesson._lessonId) as Expander;
                        }
                    }
                }
            }

            return _userInfo;
        }

        private void ClearGridCels(Grid grid)
        {
            List<UIElement> gridRemovingChilds = new List<UIElement>();

            foreach (UIElement control in grid.Children)
            {
                if (Grid.GetRow(control) < 3 && Grid.GetColumn(control) > 0)
                {
                    gridRemovingChilds.Add(control);
                }
                else if(Grid.GetColumn(control) == 0  && Grid.GetRow(control) != 3)
                {
                    RadioButton rb = (RadioButton)control;
                    if (Grid.GetRow(control) != 0)
                    {
                        rb.IsEnabled = false;
                    }
                    else
                    {
                        rb.IsChecked = true;
                    }
                }
            }
            foreach (var element in gridRemovingChilds)
            {
                grid.Children.Remove(element);
            }
        }

        private void ShowMessageBox(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("К сожалению, здесь нет ничего! Зато есть 2 рабочих урока: 'Обезьянка на прогулке' и 'Цветочная поляна'.");
        }

        private void UpdateGamesGrid(object sender, SizeChangedEventArgs e)
        {
            tbctrlLessons.Height = this.ActualHeight - 120;
        }
    }
}
