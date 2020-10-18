using System;
using System.ComponentModel;
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
using System.Windows.Resources;
using System.Windows.Threading;
using System.Threading;

namespace Razvivashka
{
    /// <summary>
    /// Interaction logic for Lesson.xaml
    /// </summary>
    public partial class LessonWindow : Window
    {
        public UserInfo _user;
        public int _lesson;
        public int _level;
        string resourcesPath = XmlFunctions.path + "//Resourses";

        private DispatcherTimer timer = null;
        private int _min = 0;
        private int _sec = 0;

        private int _windWidth;
        private int _windHeight;

        private int _counter;

        public LessonWindow(UserInfo user, string lessonName, int level)
        {
            InitializeComponent();
            _user = user;
            _lesson = Convert.ToInt32(lessonName.Substring(lessonName.Length - 1));
            _level = level;

            lbLessonName.Content = "Урок \"" + lessonName + "\", уровень " + _level;

            this.SizeChanged += OnWindowSizeChanged;

            //Button btnStart = new Button()
            //{
            //    Content = "Начать занятие!",
            //    Name = "btnStart",
            //    Height = 40,
            //    Width = 200
            //};
            //btnStart.Click += StartGame;

            Button btnStart = CreateBtn("Начать занятие!", "btnStart", 40, 200, StartGame);
            cnvLesson.Children.Add(btnStart);

            Canvas.SetTop(btnStart, (this.Height - 120) / 2);
            Canvas.SetLeft(btnStart, (this.Width - 220) / 2);
        }

        public Button CreateBtn(string content, string name, int height = 20, int width = 120, RoutedEventHandler function = null)
        {
            Button btn = new Button()
            {
                Content = content,
                Name = name,
                Height = height,
                Width = width
            };
            if (function != null)
            {
                btn.Click += function;
            }
            return btn;
        }

        //здесь добавить метод внесения в БД
        public void StartGame(object sender, RoutedEventArgs e)
        {
            lbLessonName.Content = lbLessonName.Content.ToString().Replace("уровень "+(_level-1), "уровень " + (_level));

            Button btn = sender as Button;
            dckpLesson.Children.Remove(btn);
            WindowClear();
            GetCnvSize();
            //_counter = 10 * _level;
            _counter = 1 * _level;
            timerStart();
            switch (_lesson)
            {
                case 1:
                    MessageBox.Show("Лабиринт с мышкой");
                    break;
                case 2:
                    CatchButterfly(sender, e);
                    break;
                case 3:
                    MessageBox.Show("Помоги мышке добраться до сыра");
                    break;
            }
        }

        public void NextLevel(object sender, RoutedEventArgs e)
        {
            _level++;
            StartGame(sender, e);
        }

        public void CatchButterfly(object sender, RoutedEventArgs e)
        {
            cnvLesson.Children.Remove(sender as Button);

            if (_counter-- > 0)
            {
                AddButterfly();
            }
            else
            {
                timer.Stop();
                WriteResultIntoXML();
                MessageBox.Show("Ты прошел уровень! Твоё время: " + lbTimer.Content);
                wpButtonsBottom.Children.Add(CreateBtn("Пройти еще раз", "btnAgain", function: StartGame));
                if (_level < 3)
                {
                    wpButtonsBottom.Children.Add(CreateBtn("Далее", "btnNext", function: NextLevel));
                }
            }
        }

        private void WriteResultIntoXML()
        {
            XmlFunctions.UpdateXmlResult(_user.Name, _lesson, _level, lbTimer.Content.ToString());
        }

        private void GetCnvSize()
        {
            try
            {
                _windWidth = Convert.ToInt32(cnvLesson.ActualWidth);
                _windHeight = Convert.ToInt32(cnvLesson.ActualHeight);
            }
            catch (Exception)
            {
                _windWidth = 0;
                _windHeight = 0;
            }
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GetCnvSize();
        }

        public void AddButterfly()
        {
            Button btnBatefly = new Button()
            {
                Name = "btnBatefly",
                Width = 30 * (4 - _level),
                Height = 40 * (4 - _level),
            };
            btnBatefly.Click += CatchButterfly;
            btnBatefly.Style = (Style)this.TryFindResource("DefaultBtn");

            cnvLesson.Children.Add(btnBatefly);

            Random rand = new Random();
            Canvas.SetLeft(btnBatefly, rand.Next(0, _windWidth - Convert.ToInt32(btnBatefly.Width)));
            Canvas.SetTop(btnBatefly, rand.Next(0, _windHeight - Convert.ToInt32(btnBatefly.Height)));
        }

        private void WindowClear()
        {
            _min = 0;
            _sec = 0;
            lbTimer.Content = String.Format("{0:00}:{1:00}", _min, _sec);

            wpButtonsBottom.Children.Clear();
        }

        private void timerStart()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (_sec == 59)
            {
                _min += 1;
                _sec = 0;
            }
            else
            {
                _sec += 1;
            }
            lbTimer.Content = String.Format("{0:00}:{1:00}", _min, _sec);
        }
    }
}
