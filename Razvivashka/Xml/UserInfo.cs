using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Razvivashka.Xml
{
    public class UserInfo
    {
        public string Name;
        public List<Lesson> Lessons;
    }

    public class Lesson
    {
        public int _lessonId;
        public List<string> _levelTime;

        public Lesson() { }

        public Lesson(int LessonId, List<string> LevelTime)
        {
            _lessonId = LessonId;
            _levelTime = LevelTime;
        }
    }
}
