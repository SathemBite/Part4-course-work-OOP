using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    public enum Level
    {
        LOW,
        MIDDLE,
        ADVANCED,
        HIGH
    }

    public enum Language
    {
        ENGLISH,
        GERMAN,
        SPANISH,
        FRENCH,
        RUSSIAN,
        JAPANESE
    }

    public enum Intensity
    {
        STANDARD,
        MAINTAINING,
        INTENSIVE
    }

    public enum DayOfWeek
    {
        MON,
        TUE,
        WED,
        THU,
        FRI
    }

    public enum ClassTime
    {
        MORNING,
        AFTERNOON,
        EVENING
    }


    class Courses
    {
        private static Dictionary<Language, LinkedList<Group>> groupC;
        private static Dictionary<Language, LinkedList<Student>> individualC;
        private static Dictionary<Language, LinkedList<Student>> individualWishGroupC;
        private static Random rand;

        static Courses()
        {
            groupC = new Dictionary<Language, LinkedList<Group>>();
            individualC = new Dictionary<Language, LinkedList<Student>>();
            individualWishGroupC = new Dictionary<Language, LinkedList<Student>>();
            rand = new Random();
        }

        public static bool isCanFormGroup(LinkedList<DayOfWeek> visDays, Language lang, Level level, Intensity inten) {
            LinkedList<Student> wishGroupStudents; 
            bool isGrExist = individualC.TryGetValue(lang, out wishGroupStudents);

            return isGrExist 
                ? wishGroupStudents.Where(st => st.containCourse(visDays, lang, level, inten)).Count() >= 5
                : false;
        }




        public static void addStudent(Student addingStudent)
        {
            Group studGr;
            foreach (KeyValuePair<Language, CourseInf> course in addingStudent.courses)
            {
                if (course.Value.isWishGroup)
                {
                    LinkedList<Student> wishGroupSt;
                    if (individualWishGroupC.TryGetValue(course.Key, out wishGroupSt))
                    {
                        wishGroupSt.AddLast(addingStudent);
                    }
                    else
                    {
                        wishGroupSt = new LinkedList<Student>();
                        wishGroupSt.AddLast(addingStudent);
                        individualWishGroupC.Add(course.Key, wishGroupSt);
                    }
                }
                else
                {
                    LinkedList<Student> indivClassesSt;
                    if (individualC.TryGetValue(course.Key, out indivClassesSt)) 
                    {
                        indivClassesSt.AddLast(addingStudent);
                    }
                    else
                    {
                        indivClassesSt = new LinkedList<Student>();
                        indivClassesSt.AddLast(addingStudent);
                        individualC.Add(course.Key, indivClassesSt);
                    }
                }
            }
        }

        /*public static Student GenerateStudent()
        {
            String[] fNames = Properties.Resources.CSV_Database_of_First_Names.Split('\u000D');
            String[] lNames = Properties.Resources.CSV_Database_of_Last_Names.Split('\u000D');
            int getCountOfListeners()OfFNames = fNames.Length - 1;
            int getCountOfListeners()OfLNames = lNames.Length - 1;
            int age = GenerateAge();
            Level level = generateLevel(age);
            return new Student(
                fNames[rand.Next(getCountOfListeners()OfFNames) + 1] + " " + lNames[rand.Next(getCountOfListeners()OfLNames) + 1],
                age, 
                generateLanguage(),
                level,
                generateIntensivity(level));
        }*/

        private static int GenerateAge()
        {
            int randomNum = rand.Next(10);

            if (randomNum < 3)
                return 7 + rand.Next(10);

            if (randomNum < 7)
                return 18 + rand.Next(12);

            if (randomNum < 9)
                return 31 + rand.Next(20);

            return 52 + rand.Next(48);
        }

        public static Level generateLevel(int age)
        {
            int eval = age < 18 
                ? rand.Next(100) + 40 - age * 2
                : rand.Next(100);

            if (eval < 10)
                return Level.HIGH;
            if (eval < 25)
                return Level.ADVANCED;
            if (eval < 55)
                return Level.MIDDLE;

            return Level.LOW;
        }

        public static Intensity generateIntensivity(Level level)
        {
            int eval = rand.Next(100);

            switch (level)
            {
                case Level.LOW:
                    return eval < 40
                        ? Intensity.STANDARD
                        : Intensity.INTENSIVE;
                case Level.MIDDLE:
                    return eval < 15
                        ? Intensity.MAINTAINING
                        : eval < 70
                        ? Intensity.STANDARD
                        : Intensity.INTENSIVE;
                case Level.ADVANCED:
                    return eval < 15
                        ? Intensity.INTENSIVE
                        : eval < 60
                        ? Intensity.STANDARD
                        : Intensity.MAINTAINING;
                case Level.HIGH:
                    return eval < 10
                        ? Intensity.INTENSIVE
                        : eval < 40
                        ? Intensity.STANDARD
                        : Intensity.MAINTAINING;
                default: throw new ArgumentException(); 
            }
        }

        public static Language generateLanguage(){
            int eval = rand.Next(100);

            return eval < 50
                ? Language.ENGLISH
                : eval < 70
                ? Language.GERMAN
                : eval < 80
                ? Language.SPANISH
                : eval < 87
                ? Language.FRENCH
                : eval < 95
                ? Language.RUSSIAN
                : Language.JAPANESE;
        }

        public static int getTwoWeekCost(Language lang, Intensity intens, bool isGroup, int days)
        {
            double cost = 0;
            switch (lang)
            {
                case Language.ENGLISH: cost = 15; break;
                case Language.GERMAN: cost = 17; break;
                case Language.FRENCH: cost = 19; break;
                case Language.SPANISH: cost = 21; break;
                case Language.RUSSIAN: cost = 23; break;
                case Language.JAPANESE: cost = 25; break;
                default: cost = 0; break;
            }
            if (intens == Intensity.INTENSIVE)
                cost *= 1.5;
            if (intens == Intensity.MAINTAINING)
                cost *= 0.5;

            if (!isGroup)
            {
                cost *= 2.5;
            }

            return (int) (cost * days);
        }

        private static LinkedList<DayOfWeek> generateVisitDaysOfWeek()
        {
            LinkedList<DayOfWeek> days = new LinkedList<DayOfWeek>();
            Random rand = new Random();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek))){
                if (rand.NextDouble() < 0.6)
                {
                    days.AddLast(day);
                }
            }
            if (days.Count == 0)
            {
                days.AddLast((DayOfWeek)rand.Next(6));
            }
            return days;
        }

        public void regroup(LinkedList<Group> groups)
        {
            groups.OrderBy(group => group.getCountOfListeners());
            while (groups.Last().getCountOfListeners() - groups.First().getCountOfListeners() > 1)
            {
                Group.moveStudents(
                    groups.Last(),
                    groups.First(),
                    Math.Min(Math.Abs(groups.Last().getCountOfListeners() - 7), Math.Abs(groups.First().getCountOfListeners() - 7)));
                groups.OrderBy(group => group.getCountOfListeners());
            }
        }

    }
}
