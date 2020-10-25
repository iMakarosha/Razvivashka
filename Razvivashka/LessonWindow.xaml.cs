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

            switch (_lesson)
            {
                case 1:
                    tbLessonDescr.Text = "Веди обезьянку по тропинке через джунгли от старта до финиша! Голова обезьяны должна быть на тропинке.";
                    lessonName = "Обезьянка на прогулке";
                    break;
                case 2:
                    tbLessonDescr.Text = "Наведи курсор на большой белый цветок и кликни по нему мышкой, чтоб помочь бабочке собрать нектар!";
                    lessonName = "Цветочная поляна";
                    break;
                case 3:
                    tbLessonDescr.Text = "";
                    lessonName = "";
                    break;
            }
            lbLessonName.Content = "Урок \"" + lessonName + "\", уровень " + _level;

            this.SizeChanged += OnWindowSizeChanged;

            dckpLesson.Style = (Style)TryFindResource("lessonBack" + _lesson);

            Button btnStart = CreateBtn("Начать занятие!", "btnStart", 60, 260, StartGame, "lessonBtn");
            cnvLesson.Children.Add(btnStart);

            Canvas.SetTop(btnStart, (this.Height - 160) / 2);
            Canvas.SetLeft(btnStart, (this.Width - 260) / 2);
        }

        public Button CreateBtn(string content, string name, int height = 0, int width = 0, RoutedEventHandler function = null, string btnStyle = null)
        {
            Button btn = new Button()
            {
                Content = content,
                Name = name
            };
            if (height > 0 && width > 0)
            {
                btn.Height = height;
                btn.Width = width;
                btn.FontSize = 18;
            }
            if (function != null)
            {
                btn.Click += function;
            }
            if (!String.IsNullOrEmpty(btnStyle))
            {
                btn.Style = (Style)TryFindResource(btnStyle);
            }
            return btn;
        }

        //здесь добавить метод внесения в БД
        public void StartGame(object sender, RoutedEventArgs e)
        {
            lbLessonName.Content = lbLessonName.Content.ToString().Replace("уровень "+(_level-1), "уровень " + (_level));

            WindowClear();
            Button btn = sender as Button;
            dckpLesson.Children.Remove(btn);
            GetCnvSize();
            _counter = 10 * _level;
            timerStart();
            switch (_lesson)
            {
                case 1:
                    StartWalk(sender, e);
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
                wpButtonsBottom.Children.Add(CreateBtn("Пройти еще раз", "btnAgain", function: StartGame, btnStyle: "lessonBtn"));
                if (_level < 3)
                {
                    wpButtonsBottom.Children.Add(CreateBtn("Далее", "btnNext", function: NextLevel, btnStyle: "lessonBtn"));
                }
            }
        }

        public void StartWalk(object sender, RoutedEventArgs e)
        {

            if (_counter > 0)//другое
            {
                cnvLesson.Children.Remove(sender as Button);
                GenerateNewPath(_counter/10!=2);
            }
            else
            {
                timer.Stop();
                WriteResultIntoXML();
                MessageBox.Show("Ты прошел уровень! Твоё время: " + lbTimer.Content);
                wpButtonsBottom.Children.Add(CreateBtn("Пройти еще раз", "btnAgain", function: StartGame, btnStyle: "lessonBtn"));
                if (_level < 3)
                {
                    wpButtonsBottom.Children.Add(CreateBtn("Далее", "btnNext", function: NextLevel, btnStyle: "lessonBtn"));
                }
            }
        }

        public void GenerateNewPath(bool revertPath)
        {
            int thinckness = 6 / _level * 12;
            Point[] points = new Point[4];
            int startIndex = revertPath ? 4 : 0;
            for(int i = 0; i < 4; i++)
            {
                points[i] = GetNewRandomPoint(thinckness, ++startIndex);
            }
            PathFigure pf = new PathFigure()
            {
                Segments = new PathSegmentCollection() { new BezierSegment(points[1], points[2], points[3], true) },
                StartPoint = points[0]
            };

            Path walkPath = new Path()
            {
                Name = "walkPath",
                StrokeThickness = thinckness,
                Style = (Style)this.TryFindResource("DefaultPath"),
                Data = new PathGeometry() { Figures = new PathFigureCollection() { pf } }
            };
            Path firstPoint = new Path()
            {
                Fill = Brushes.BurlyWood,
                Style = (Style)this.TryFindResource("DefaultPath"),
                Data = new EllipseGeometry(points[0], thinckness / 2, thinckness / 2)
            };
            Path lastPoint = new Path()
            {
                //Fill = new SolidColorBrush(Color.FromRgb(255,231,215)),
                Fill = Brushes.BurlyWood,
                Style = (Style)this.TryFindResource("DefaultPath"),
                Data = new EllipseGeometry(points[3], thinckness / 2, thinckness / 2)
            };
            Button upArrowBtn = CreateBtn("", "upArrowBtn", btnStyle: "upBtn");
            Button homeBtn = CreateBtn("", "homeBtn", btnStyle: "homeBtn");
            walkPath.MouseLeave += Polygon_MouseLeave;
            walkPath.IsEnabled = false;

            firstPoint.MouseMove += FirstPoint_MouseEnter;
            //lastPoint.MouseMove += LastPoint_MouseEnter;

            cnvLesson.Children.Add(firstPoint);
            cnvLesson.Children.Add(lastPoint);
            cnvLesson.Children.Add(walkPath);
            cnvLesson.Children.Add(upArrowBtn);
            cnvLesson.Children.Add(homeBtn);
            Canvas.SetLeft(upArrowBtn, points[0].X);
            Canvas.SetTop(upArrowBtn, points[0].Y + 30);

            Canvas.SetTop(homeBtn, points[3].Y - (80 + thinckness/2));
            if (revertPath)
            {
                Canvas.SetLeft(homeBtn, points[3].X - 30);
            }
            else
            {
                Canvas.SetLeft(homeBtn, points[3].X - 50);
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
                if(_lesson == 1)
                {
                    _windWidth = Convert.ToInt32(cnvLesson.ActualWidth - (6 / _level * 10));
                    _windHeight = Convert.ToInt32(cnvLesson.ActualHeight - (6 / _level * 8));
                }
                else
                {
                    _windWidth = Convert.ToInt32(cnvLesson.ActualWidth);
                    _windHeight = Convert.ToInt32(cnvLesson.ActualHeight);
                }
            }
            catch (Exception)
            {
                _windWidth = 0;
                _windHeight = 0;
            }
        }

        private Point GetNewRandomPoint(int thinckness, int pointNumber)
        {
            int squareW = Convert.ToInt32(_windWidth / 6);
            int squareH = Convert.ToInt32(_windHeight / 4);
            switch (pointNumber)
            {
                case 1:
                    return new Point(
                        GetNewRandL1(squareW, 0, 60),
                        GetNewRandL1(squareH, minParam: 60)
                        );
                case 2:
                    return new Point(
                        GetNewRandL1(_windWidth, 0, squareW * 5) * 2,
                        GetNewRandL1(squareH, minParam: 60)
                        );
                case 3:
                    return new Point(
                        GetNewRandL1(squareW)*(-8),
                        GetNewRandL1(_windHeight, 60, squareH * 3)
                        );
                case 4:
                    return new Point(
                        GetNewRandL1(_windWidth, 0, squareW * 5),
                        GetNewRandL1(_windHeight, 30, squareH * 3)
                        );
                //revert
                case 5:
                    return new Point(
                        GetNewRandL1(_windWidth, 0, squareW * 5),
                        GetNewRandL1(squareH, minParam: 60)
                        );
                case 6:
                    return new Point(
                        GetNewRandL1(squareW) * (-8),
                        GetNewRandL1(squareH, minParam: 60)
                        );
                case 7:
                    return new Point(
                        GetNewRandL1(_windWidth, 0, squareW * 5) * 2,
                        GetNewRandL1(_windHeight, 60, squareH * 3)
                        );
                default:
                    return new Point(
                        GetNewRandL1(squareW, 0, 60),
                        GetNewRandL1(_windHeight, 30, squareH * 3)
                        );
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
                Width = 40 * (4 - _level),
                Height = 34 * (4 - _level),
                Style = (Style)this.TryFindResource("DefaultBtn")
            };
            btnBatefly.Click += CatchButterfly;
            cnvLesson.Children.Add(btnBatefly);

            Canvas.SetLeft(btnBatefly, GetNewRandL2(_windWidth, btnBatefly.Width));
            Canvas.SetTop(btnBatefly, GetNewRandL2(_windHeight, btnBatefly.Height));
        }

        private double GetNewRandL1(double maxParam, double maxParamSubstr = 0, double minParam = 0)
        {
            return new Random().Next(Convert.ToInt32(minParam), Convert.ToInt32(maxParam) - Convert.ToInt32(maxParamSubstr));
        }


        private double GetNewRandL2(double maxParam, double maxParamSubstr = 0, double minParam = 0)
        {
            int random = new Random().Next(Convert.ToInt32(minParam), Convert.ToInt32(maxParam) - Convert.ToInt32(maxParamSubstr));
            int isRefresh = new Random().Next(1, 8);
            if (isRefresh % 2 == 1) {
                try
                {
                    return Convert.ToInt32(new Random().Next(Convert.ToInt32(minParam + 5), Convert.ToInt32(_windHeight - 100)));
                }
                catch(Exception ex)
                {
                    return new Random().Next(Convert.ToInt32(minParam), Convert.ToInt32(maxParam) - Convert.ToInt32(maxParamSubstr));
                }
            }
            return random;
        }

        private void WindowClear()
        {
            _min = 0;
            _sec = 0;
            lbTimer.Content = String.Format("{0:00}:{1:00}", _min, _sec);

            wpButtonsBottom.Children.Clear();
            cnvLesson.Children.Clear();
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

        private void Polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            Path lastPoint = XmlFunctions.FindVisualChildren<Path>(cnvLesson, 1);
            if (lastPoint.IsMouseOver)
            {
                _counter = 0;
                foreach (Path path in XmlFunctions.FindVisualChildren<Path>(cnvLesson))
                {
                    path.IsEnabled = false;
                }
                StartWalk(sender, e);
            }
            else
            {
                DisablePath();
                MessageBox.Show("Попробуй еще раз!");

            }
        }

        private void FirstPoint_MouseEnter(object sender, MouseEventArgs e)
        {
            EnablePath();
        }

        private void EnablePath()
        {
            Path walkPath = XmlFunctions.FindVisualChildren<Path>(cnvLesson, 2);
            walkPath.IsEnabled = true;
        }

        private void DisablePath()
        {
            Path walkPath = XmlFunctions.FindVisualChildren<Path>(cnvLesson, 2);
            walkPath.IsEnabled = false;
        }
    }
}
