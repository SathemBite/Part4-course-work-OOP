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
            bool isSuchStudentsExist = individualWishGroupC.TryGetValue(lang, out wishGroupStudents);

            return isSuchStudentsExist
                ? wishGroupStudents.Where(st => st.containsCourse(visDays, lang, level, inten)).Count() >= 4
                : false;
        }

        public static void formGroups()
        {
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                LinkedList<Student> specLangSt;
                if (individualWishGroupC.TryGetValue(lang, out specLangSt))
                {
                    if (specLangSt.Count < 5)
                    {
                        continue;
                    }
                    foreach (Level level in Enum.GetValues(typeof(Level)))
                    {
                        if (specLangSt.Any(st => st.containsCourse(level))){
                            LinkedList<Student> specLevelSt = 
                                new LinkedList<Student>(specLangSt.Where(st => st.containsCourse(level)));
                            if (specLevelSt.Count < 5)
                            {
                                continue;
                            }
                            foreach (Intensity inten in Enum.GetValues(typeof(Intensity)))
                            {
                                if (specLevelSt.Any(st => st.containsCourse(inten))){
                                    LinkedList<Group> existGr;
                                    IEnumerable<Student> evenDaysSt = specLangSt.Where(
                                        st => st.containsCourse(DayOfWeek.TUE, inten));
                                    IEnumerable<Student> conDaysSt = specLangSt.Where(
                                        st => st.containsCourse(DayOfWeek.MON, inten));
                                    LinkedList<Student> sts = new LinkedList<Student>(conDaysSt.ToList());
                                    if (groupC.TryGetValue(lang, out existGr))
                                    {
                                        if (conDaysSt.Count() >= 5)
                                        {
                                            existGr = new LinkedList<Group>(existGr.Concat(formSpecGroups(new LinkedList<Student>(conDaysSt.ToList()), lang, level, inten, getConWDays()).ToList()));
                                        }
                                        if (evenDaysSt.Count() >= 5)
                                        {
                                            existGr = new LinkedList<Group>(existGr.Concat(formSpecGroups(new LinkedList<Student>(evenDaysSt.ToList()), lang, level, inten, getEvenWDays()).ToList()));
                                        }
                                    }
                                    else
                                    {
                                        if (conDaysSt.Count() >= 5)
                                        {
                                            existGr = formSpecGroups(new LinkedList<Student>(conDaysSt.ToList()), lang, level, inten, getConWDays());
                                        }
                                        if (evenDaysSt.Count() >= 5)
                                        {
                                            existGr = formSpecGroups(new LinkedList<Student>(evenDaysSt.ToList()), lang, level, inten, getEvenWDays());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }             
            }
        }

        public static LinkedList<DayOfWeek> getConWDays()
        {
            LinkedList<DayOfWeek> days = new LinkedList<DayOfWeek>();
            days.AddLast(DayOfWeek.MON);
            days.AddLast(DayOfWeek.WED);
            days.AddLast(DayOfWeek.FRI);
            return days;
        }

        public static LinkedList<DayOfWeek> getEvenWDays()
        {
            LinkedList<DayOfWeek> days = new LinkedList<DayOfWeek>();
            days.AddLast(DayOfWeek.TUE);
            days.AddLast(DayOfWeek.THU);
            return days;
        }

        public static LinkedList<Group> formSpecGroups(
            LinkedList<Student> students, Language lang, 
            Level level, 
            Intensity inten, 
            LinkedList<DayOfWeek> visDays)
        {
            int countOfSpecGroups = students.Count / 10 + students.Count % 10 == 0 ? 0 : 1;
            LinkedList<Group> res = new LinkedList<Group>();

            for (int i = 0; i < countOfSpecGroups; i++)
            {
                Group temp = new Group(lang, level, inten, visDays);
                for (int j = 0; j < 5; j++)
                {
                    temp.addStudent(students.First());
                    Student s = students.First();
                    students.RemoveFirst();
                    students.AddLast(s);
                }
                res.AddLast(temp);
            }

            int remStud = students.Count - countOfSpecGroups * 5;

            for (int i = 0; i < remStud; i++)
            {
                res.First().addStudent(students.First());
                Group temp = res.First();
                res.RemoveFirst();
                res.AddLast(temp);
                Student s = students.First();
                students.RemoveFirst();
                students.AddLast(s);
            }

            return res;
        }




        public static void addStudent(Student addingStudent)
        {
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

        public static Student GenerateStudent()
        {
            String[] fNames = Properties.Resources.CSV_Database_of_First_Names.Split('\u000D');
            String[] lNames = Properties.Resources.CSV_Database_of_Last_Names.Split('\u000D');
            int getCountOfListenersOfFNames = fNames.Length - 1;
            int getCountOfListenersOfLNames = lNames.Length - 1;
            int age = GenerateAge();
            Level level = generateLevel(age);
            return new Student(
                fNames[rand.Next(getCountOfListenersOfFNames) + 1] + " " + lNames[rand.Next(getCountOfListenersOfLNames) + 1],
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
                cost *= 1.25;
            if (intens == Intensity.MAINTAINING)
                cost *= 0.75;

            if (!isGroup)
            {
                cost *= 3;
            }

            return (int) (cost * days);
        }

        private static LinkedList<DayOfWeek> generateVisitDaysOfWeek(bool isGroup)
        {
            LinkedList<DayOfWeek> days = new LinkedList<DayOfWeek>();
            if (isGroup)
            {

            }
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

        public static bool isLeaveCourse(int classNum)
        {
            double arg = (double) 1 / Math.Pow(2, classNum);
            Random random = new Random();
            double rand = random.NextDouble() * 2;
            return (1 / (1 + Math.Pow(Math.E, arg))) > rand;

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
