using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Razvivashka.Xml
{
    public static class XmlFunctions
    {
        //static string xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Data/Users.xml";
        public static string path = "C://Users//IrinaUser1//source//repos//Razvivashka//Razvivashka";
        //static string xmlPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Data/Users.xml";
        public static string xmlPath = path+"//DataExample//Users.xml";

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
                                if (childnode.Name == "Lesson")
                                {
                                    Lesson lesson = new Lesson();

                                    if (childnode.Attributes.Count > 0)
                                    {
                                        XmlNode levelId = childnode.Attributes.GetNamedItem("id");
                                        if (levelId != null)
                                            lesson._lessonId = Convert.ToInt32(levelId.Value) ;
                                    }

                                    List<string> levels = new List<string>();
                                    foreach (XmlNode levelInfo in childnode.ChildNodes)
                                    {
                                        if(levelInfo.Name == "Level")
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

        public static List<UserInfo> XmlReader(bool forStatistic)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath); 
            XmlElement xRoot = xmlDoc.DocumentElement;
            List<UserInfo> users = new List<UserInfo>();
            foreach (XmlNode xnode in xRoot)
            {
                UserInfo info = new UserInfo();
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("name");
                    if (attr != null)
                        info.Name = attr.Value;
                }

                //foreach (XmlNode childnode in xnode.ChildNodes)
                //{
                //    if (childnode.Name == "age")
                //    {
                //        info.Age = Convert.ToInt32(childnode.InnerText);
                //    }
                //    //...
                //}
            }
            return users;
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


        #endregion
    }
}
