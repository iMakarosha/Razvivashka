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
    public partial class Lesson : Window
    {
        public UserInfo _user;
        public string _lesson;
        public int _level;
        string resourcesPath = XmlFunctions.path + "//Resourses";

        private DispatcherTimer timer = null;
        private int _min = 0;
        private int _sec = 0;

        private int _windWidth;
        private int _windHeight;

        private int _counter;

        private Cursor lessCursor;
        private Style btnStyle;

        public Lesson(UserInfo user, string lessonName, int level)
        {
            InitializeComponent();
            _user = user;
            _lesson = lessonName;
            _level = level;

            lbLessonName.Content = "Урок \"" + _lesson + "\", уровень " + _level;

            this.SizeChanged += OnWindowSizeChanged;

            Button btnStart = new Button()
            {
                Content = "Начать занятие!",
                Name = "btnStart",
                Height = 40,
                Width = 200
            };
            btnStart.Click += StartGame;
            cnvLesson.Children.Add(btnStart);
            Canvas.SetTop(btnStart, (this.Height - 120) / 2);
            Canvas.SetLeft(btnStart, (this.Width - 220) / 2);
            lbTimer.Content = String.Format("{0:00}:{1:00}", _min, _sec);
        }

        //здесь добавить метод внесения в БД
        public void StartGame(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            dckpLesson.Children.Remove(btn);

            GetCnvSize();
            _counter = 10 * _level;
            timerStart();
            switch (_lesson)
            {
                case "ExpLesson1":
                    MessageBox.Show("Лабиринт с мышкой");
                    break;
                case "ExpLesson2":
                    //lessCursor= new Cursor(new System.IO.MemoryStream(Razvivashka.Properties.Resources.Butterfly_cursor));

                    btnStyle = new Style(typeof(Button));
                    btnStyle.Setters.Add(new Setter { Property = Control.BorderBrushProperty, Value = Brushes.Transparent });
                    btnStyle.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = new ImageBrush(new BitmapImage(new Uri(resourcesPath + "//flower.png"))) });
                    //btnStyle.Setters.Add(new Setter { Property = Control.CursorProperty, Value = lessCursor });
                    btnStyle.Setters.Add(new EventSetter { Event = Button.ClickEvent, Handler = new RoutedEventHandler(CatchButterfly) });
                    btnStyle.Setters.Add(new Setter { Property = Control.OpacityProperty, Value = 0.8 });

                    Trigger tg = new Trigger()
                    {
                        Property = Button.IsMouseOverProperty,
                        Value = true
                    };
                    tg.Setters.Add(new Setter { Property = Control.CursorProperty, Value = lessCursor });
                    tg.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new FontFamily("Verdana") });
                    tg.Setters.Add(new Setter { Property = Control.OpacityProperty, Value = 1.0 });
                    btnStyle.Triggers.Add(tg);

                    CatchButterfly(sender, e);
                    break;
                case "ExpLesson3":
                    MessageBox.Show("Помоги мышке добраться до сыра");
                    break;
            }
        }

        public void CatchButterfly(object sender, RoutedEventArgs e)
        {
            if (_counter-- > 0)
            {
                cnvLesson.Children.Remove(sender as Button);
                AddButterfly();
            }
            else
            {
                MessageBox.Show("END!");
                timer.Stop();

            }
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

            //btnStyle.Setters.Add(new Setter { Property = Control.FontFamilyProperty, Value = new FontFamily("Verdana") });
            //btnStyle.Setters.Add(new Setter { Property = Control.MarginProperty, Value = new Thickness(10) });

            //Trigger trigger = new Trigger();
            //trigger.Property = Button.IsMouseOverProperty;
            //trigger.Value = true;
            //trigger.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = Brushes.Red });

            //btnStyle.Triggers.Add(trigger);

            Button btnBatefly = new Button()
            {
                Name = "btnBatefly",
                Content="sdfaas",
                Width = 30 * (4 - _level),
                Height = 40 * (4 - _level),
                //Cursor = lessCursor,
                //Background = new ImageBrush(new BitmapImage(new Uri(resourcesPath + "//flower.png")))
            };
            //btnBatefly.Style = btnStyle;
            btnBatefly.Click += CatchButterfly;

            //tg.Setters.Add(new Setter()
            //{
            //    Property = Button.MarginProperty,
            //    Value = 30
            //});

            //btnBatefly.Style = btnStyle;
            btnBatefly.Style = (Style)this.TryFindResource("DefaultBtn");
            //btnBatefly.Style = (Style)this.TryFindResource("sadfsa");
            cnvLesson.Children.Add(btnBatefly);

            Random rand = new Random();
            Canvas.SetLeft(btnBatefly, rand.Next(0, _windWidth - Convert.ToInt32(btnBatefly.Width)));
            Canvas.SetTop(btnBatefly, rand.Next(0, _windHeight - Convert.ToInt32(btnBatefly.Height)));
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
