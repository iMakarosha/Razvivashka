using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Media;

namespace Razvivashka.Xml
{
    public static class XmlFunctions
    {
        //static string xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Data/Users.xml";
        public static string path = "E://Учеба//Человеко-машинный интерфейс//Razvivashka//Razvivashka";
        //static string xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Data/Users.xml";
        public static string xmlPath = path+"//DataExample//Users.xml";

        public class XmlLevels
        {
            public int LevelId;
            public XmlNode LevelNode;
        }

        #region Reader
        public static List<string> XmlReader()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlElement xRoot = xmlDoc.DocumentElement;
            List<string> users = new List<string>();
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    if (attr != null)
                        users.Add(attr.Value);
                }
            }
            return users;
        }

        public static UserInfo XmlReader(string userName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath) ;
            XmlElement xRoot = xmlDoc.DocumentElement;
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    if (attr != null)
                    {
                        if (attr.Value == userName)
                        {
                            UserInfo info = new UserInfo();
                            info.Name = attr.Value;
                            List<Lesson> lessons = new List<Lesson>();
                            foreach (XmlNode childnode in xnode.ChildNodes)
                            {
                                if (childnode.Name == "lesson")
                                {
                                    Lesson lesson = new Lesson();

                                    if (childnode.Attributes.Count > 0)
                                    {
                                        XmlNode levelId = childnode.Attributes.GetNamedItem("name");
                                        if (levelId != null)
                                            lesson._lessonId = Convert.ToInt32(levelId.Value) ;
                                    }

                                    List<string> levels = new List<string>();
                                    foreach (XmlNode levelInfo in childnode.ChildNodes)
                                    {
                                        if(levelInfo.Name == "level")
                                        {
                                            levels.Add(levelInfo.InnerText.ToString());
                                        }
                                    }
                                    if (levels.Count > 0)
                                    {
                                        lesson._levelTime = levels;
                                        lessons.Add(lesson);
                                    }
                                }
                            }
                            info.Lessons = lessons.Count > 0 ? lessons : null;
                            return info;
                        }
                    }
                }
            }
            return null;
        }

        public static XmlNode XmlReader(string userName, XmlElement xRoot)
        {
            foreach (XmlNode xnode in xRoot)
            {
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    if (attr != null && attr.Value == userName)
                    {
                        return xnode;
                    }
                }
            }
            return null;
        }

        public static List<StatisticLesson> XmlReader(bool forStatistic)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath); 
            XmlElement xRoot = xmlDoc.DocumentElement;

            List<StatisticLesson> lessons = new List<StatisticLesson>();
            for(int i = 1; i <= 3; i++)
            {
                StatisticLesson lesson = new StatisticLesson()
                {
                    _lessonId = i,
                    _usersInfo = new List<StatisticUser>()
                };
                lessons.Add(lesson);
            }
            foreach (XmlNode xnode in xRoot)
            {
                string userName = "";
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    if (attr != null)
                        userName = attr.Value;
                }

                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "lesson")
                    {
                        StatisticUser statisticUser = new StatisticUser();
                        statisticUser._name = userName;

                        TimeSpan timeSpan = new TimeSpan();
                        foreach (XmlNode levelInfo in childnode.ChildNodes)
                        {
                            if (levelInfo.Name == "level")
                            {
                                timeSpan = timeSpan.Add(new TimeSpan(0, Convert.ToInt32(levelInfo.InnerText.ToString().Split(':')[0]), Convert.ToInt32(levelInfo.InnerText.ToString().Split(':')[1])));
                            }
                        }

                        if (childnode.Attributes.Count > 0)
                        {
                            XmlNode levelId = childnode.Attributes.GetNamedItem("name");
                            if (levelId != null)
                                lessons[Convert.ToInt32(levelId.Value)-1]._usersInfo.Add(new StatisticUser()
                                {
                                    _name = userName,
                                    _commonTime = timeSpan
                                });
                        }
                    }
                }
            }

            foreach(StatisticLesson lesson in lessons)
            {
                lesson._usersInfo.Sort(delegate (StatisticUser first, StatisticUser second)
                {
                    if (first._commonTime > second._commonTime) return 1;
                    else return -1;
                });
            }
            return lessons;
        }
        #endregion

        #region Writer
        public static void AddNewUser(string userName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath); ;
            XmlElement xRoot = xmlDoc.DocumentElement;
            XmlElement userElem = xmlDoc.CreateElement("user");
            XmlAttribute nameAttr = xmlDoc.CreateAttribute("name");
            XmlText nameText = xmlDoc.CreateTextNode(userName);

            nameAttr.AppendChild(nameText);
            userElem.Attributes.Append(nameAttr);
            xRoot.AppendChild(userElem);
            xmlDoc.Save(xmlPath);
        }

        public static void UpdateXmlResult(string userName, int lessonId, int level, string time)
        {

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlElement xRoot = xmlDoc.DocumentElement;
            XmlNode currentUser = xRoot.RemoveChild(XmlReader(userName, xRoot));

            if (currentUser.ChildNodes.Count > 0){
                bool levelChanged = false;
                foreach(XmlNode lessonNode in currentUser.ChildNodes)
                {
                    if (lessonNode.Attributes.GetNamedItem("name").Value == lessonId.ToString())
                    {
                        //проще сортировать при добавлении, а следующие за непройденным чекбоксы блочить
                        List<XmlLevels> levels = new List<XmlLevels>();
                        foreach (XmlNode levelNode in lessonNode.ChildNodes)
                        {
                            if (levelNode.Attributes.GetNamedItem("name").Value != level.ToString()){
                                levels.Add(new XmlLevels
                                {
                                    LevelId = Convert.ToInt32(levelNode.Attributes.GetNamedItem("name").Value),
                                    LevelNode = levelNode
                                });
                            }
                        }
                        lessonNode.RemoveAll();

                        XmlLevels current = new XmlLevels
                        {
                            LevelId = level,
                            LevelNode = CreateLevelNode(xmlDoc, level.ToString(), time)
                        };
                        levels.Add(current);

                        levels.Sort(delegate(XmlLevels first, XmlLevels second)
                        {
                            if (first.LevelId > second.LevelId) return 1;
                            else return -1;
                        });

                        XmlAttribute lessAttr = xmlDoc.CreateAttribute("name");
                        lessAttr.AppendChild(xmlDoc.CreateTextNode(lessonId.ToString()));
                        lessonNode.Attributes.Append(lessAttr);

                        foreach (XmlLevels curLevel in levels)
                        {
                            lessonNode.AppendChild(curLevel.LevelNode);
                        }

                        levelChanged = true;
                    }
                }
                if (!levelChanged)
                {
                    currentUser.AppendChild(CreateLessonNode(xmlDoc, lessonId.ToString(), level.ToString(), time));
                }
            }
            else{
                currentUser.AppendChild(CreateLessonNode(xmlDoc, lessonId.ToString(), level.ToString(), time));
            }

            xRoot.AppendChild(currentUser);
            xmlDoc.Save(xmlPath);
        }

        public static XmlNode CreateLessonNode(XmlDocument xmlDoc, string lessonId, string levelId, string time)
        {
            XmlElement lesson = xmlDoc.CreateElement("lesson");
            XmlAttribute lessAttr = xmlDoc.CreateAttribute("name");
            lessAttr.AppendChild(xmlDoc.CreateTextNode(lessonId));
            lesson.Attributes.Append(lessAttr);

            lesson.AppendChild(CreateLevelNode(xmlDoc, levelId, time));

            return lesson;
        }

        public static XmlNode CreateLevelNode(XmlDocument xmlDoc, string levelId, string time)
        {

            XmlElement level = xmlDoc.CreateElement("level");
            XmlAttribute levAttr = xmlDoc.CreateAttribute("name");

            levAttr.AppendChild(xmlDoc.CreateTextNode(levelId));
            level.Attributes.Append(levAttr);
            level.InnerXml = time;

            return level;
        }

        #endregion



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

        public static T FindVisualChildren<T>(DependencyObject depObj, int index) where T : DependencyObject
        {
            int counter = 0;
            foreach (var el in FindVisualChildren<T>(depObj))
            {
                if (counter++ == index)
                {
                    return el;
                }
            };
            return null;
        }

    }
}
