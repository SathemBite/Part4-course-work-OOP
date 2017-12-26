using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Part4.BusinessLogic.AdaptersToDataGrid;

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

    public enum CourseStatus
    {
        LEAVED,
        ACTIVE,
        FINISHED
    }

    public struct DisplayData
    {
        public LinkedList<Group> groups;
        LinkedList<Student> students;
        LinkedList<CourseInf> courses;
    }

    class Courses
    {
        private static Dictionary<Language, List<Group>> groupC;
        private static Dictionary<Language, LinkedList<Student>> individualC;
        private static Dictionary<Language, LinkedList<Student>> individualWishGroupC;
        private static LinkedList<Group> allGroups;
        private static LinkedList<Student> allStudents;
        private static Random rand;

        private static DisplayData displayData;

        static Courses()
        {
            displayData = new DisplayData();
            groupC = new Dictionary<Language, List<Group>>();
            individualC = new Dictionary<Language, LinkedList<Student>>();
            individualWishGroupC = new Dictionary<Language, LinkedList<Student>>();
            allStudents = new LinkedList<Student>();
            allGroups = new LinkedList<Group>();
            rand = new Random();
        }

        public static bool isCanFormGroup(LinkedList<DayOfWeek> visDays, Language lang, Level level, Intensity inten) {
            LinkedList<Student> wishGroupStudents; 
            bool isSuchStudentsExist = individualWishGroupC.TryGetValue(lang, out wishGroupStudents);

            return isSuchStudentsExist
                ? wishGroupStudents.Where(st => st.containsCourse(visDays, lang, level, inten)).Count() >= 4
                : false;
        }

        public static void makeTwoWeakSteps()
        {
            foreach (Student student in allStudents)
            {
                student.makeTwoWeekStep();
            }

            foreach (KeyValuePair<Language, List<Group>> langGroups in groupC)
            {
                foreach (Group group in langGroups.Value)
                {
                    group.makeTwoWeekStep();
                }
            }
            
        }

        public static void formGroups(DataGrid groupTable, DataGrid studentTable)
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
                        if (specLangSt.Any(st => st.containsCourse(lang, level))){
                            LinkedList<Student> specLevelSt = 
                                new LinkedList<Student>(specLangSt.Where(st => st.containsCourse(lang, level)));
                            if (specLevelSt.Count < 5)
                            {
                                continue;
                            }
                            foreach (Intensity inten in Enum.GetValues(typeof(Intensity)))
                            {
                                if (specLevelSt.Any(st => st.containsCourse(lang, inten))){
                                    Dictionary<Language, List<Group>> grC = groupC;
                                    List<Group> existGr = null;
                                    IEnumerable<Student> evenDaysSt = specLevelSt.Where(
                                        st => st.containsCourse(lang, DayOfWeek.TUE, inten));
                                    IEnumerable<Student> conDaysSt = specLevelSt.Where(
                                        st => st.containsCourse(lang, DayOfWeek.MON, inten));
                                    LinkedList<Student> sts = new LinkedList<Student>(conDaysSt.ToList());
                                    if (groupC.TryGetValue(lang, out existGr))
                                    {
                                        if (conDaysSt.Count() >= 5)
                                        {
                                            existGr.AddRange(formSpecGroups(conDaysSt.ToList(), lang, level, inten, getConWDays()));
                                        }
                                        if (evenDaysSt.Count() >= 5)
                                        {
                                            existGr.AddRange(formSpecGroups(evenDaysSt.ToList(), lang, level, inten, getEvenWDays()));
                                        }
                                    }
                                    else
                                    {
                                        if (conDaysSt.Count() >= 5)
                                        {
                                            existGr = formSpecGroups(conDaysSt.ToList(), lang, level, inten, getConWDays());
                                        }
                                        if (evenDaysSt.Count() >= 5)
                                        {
                                            if (existGr != null)
                                            {
                                                existGr.AddRange(formSpecGroups(evenDaysSt.ToList(), lang, level, inten, getEvenWDays()));
                                            }
                                            else
                                            {
                                                existGr = formSpecGroups(evenDaysSt.ToList(), lang, level, inten, getEvenWDays());
                                            }
                                            
                                        }
                                        if (existGr != null)
                                        {
                                            groupC.Add(lang, existGr);
                                        }  
                                    }
                                }
                            }
                        }
                    }

                    Student[] groupSts = specLangSt.Where(st => st.isGroup(lang)).ToArray();
                    for (int i = 0; i < groupSts.Length; i++)
                    {
                            specLangSt.Remove(groupSts[i]);
                    }
                }             
            }

            List<Group> groups = getAllGroups();
            groupTable.Items.Clear();
            foreach (Group group in groups)
            {
                groupTable.Items.Add(group);
            }
            studentTable.Items.Clear();
            foreach (Student student in allStudents)
            {
                studentTable.Items.Add(student);
            }
        }

        public static void mapGroupOnStudents(DataGrid dg_groups, DataGrid dg_students)
        {
            dg_students.Items.Clear();
            Group group = (Group)dg_groups.SelectedItem;
            foreach (Student student in group.students)
            {
                dg_students.Items.Add(student);
            }
        }

        public static void mapStudentOnCourses(DataGrid dg_students, DataGrid dg_courses)
        {
            dg_courses.Items.Clear();
            Student student = (Student)dg_students.SelectedItem;
            foreach (KeyValuePair<Language, CourseInf> course in student.courses)
            {
                dg_courses.Items.Add(new CourseInform(course));
            }
        }

        public static List<Group> getAllGroups()
        {
            List<Group> groups = new List<Group>(); 

            foreach (KeyValuePair<Language, List<Group>> langGr in groupC)
            {
                groups = groups.Concat(langGr.Value).ToList();
            }

            return groups;
        }

        

        public static void addRowToGrTable(DataGrid dg, Group gr)
        {
            GroupInform tempG = new GroupInform();
            tempG.id = gr.id;
            tempG.level = gr.level.ToString();
            tempG.lang = gr.lang.ToString();
            tempG.inten = gr.inten.ToString();
            tempG.countOfListeners = gr.getCountOfListeners();
            tempG.visDays = String.Join(" ", gr.visDays.ToArray());
            dg.Items.Add(tempG);
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

        public static List<Group> formSpecGroups(
            List<Student> students, Language lang, 
            Level level, 
            Intensity inten, 
            LinkedList<DayOfWeek> visDays)
        {
            int countOfSpecGroups = students.Count / 10 + (students.Count % 10 == 0 ? 0 : 1);
            List<Group> res = new List<Group>();

            for (int i = 0; i < countOfSpecGroups; i++)
            {
                Group temp = new Group(lang, level, inten, visDays, getCourseDurationInWeeks(inten));
                for (int j = 0; j < 5; j++)
                {
                    temp.addStudent(students.First());
                    Student s = students.First();
                    students.RemoveAt(0);
                    students.Add(s);
                }
                res.Add(temp);
            }

            int remStud = students.Count - countOfSpecGroups * 5;

            for (int i = 0; i < remStud; i++)
            {
                res.First().addStudent(students.First());
                Group temp = res.First();
                res.RemoveAt(0);
                res.Add(temp);
                Student s = students.First();
                students.RemoveAt(0);
            }

            return res;
        }




        public static void addStudent(Student addingStudent)
        {
            allStudents.AddLast(addingStudent);

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

            Dictionary<Language, CourseInf> courses = generateCourses(age);
            return new Student(
                fNames[rand.Next(getCountOfListenersOfFNames) + 1] + " " + lNames[rand.Next(getCountOfListenersOfLNames) + 1],
                age, 
                courses);
        }

        public static Dictionary<Language, CourseInf> generateCourses(int age)
        {
            Dictionary<Language, CourseInf> courses = new Dictionary<Language, CourseInf>();
            KeyValuePair<Language, CourseInf> temp = generateCourse(age, null);
            courses.Add(temp.Key, temp.Value);
            Random rand = new Random();
            LinkedList<Language> availCourse = new LinkedList<Language>((Language[])Enum.GetValues(typeof(Language)));
            availCourse.Remove(temp.Key);

            for (int i = 0; i < 5; i++)
            {
                if (rand.Next(10000) <= 10001 / Math.Pow(10, i + 1))
                {
                    temp = generateCourse(age, availCourse);
                    courses.Add(temp.Key, temp.Value);
                    availCourse.Remove(temp.Key);
                }
            }
            return courses;
        }

        public static KeyValuePair<Language, CourseInf> generateCourse(int age, LinkedList<Language> availLang)
        {
            KeyValuePair<Language, CourseInf> course;
            Language lang;

            if (availLang == null)
            {
                lang = generateLanguage();
            }
            else
            {
                lang = availLang.ElementAt(new Random().Next(availLang.Count));
                availLang.Remove(lang);
            }
            
            
            Level level = generateLevel(age);
            Intensity inten = generateIntensity(level);
            bool isGroupClasses = isGroup();
            LinkedList<DayOfWeek> visDays = generateVisitDaysOfWeek(isGroupClasses);
            if (isGroupClasses)
            {
                if (isCanFormGroup(visDays, lang, level, inten))
                {
                    course = new KeyValuePair<Language, CourseInf>(lang, new CourseInf(inten, level, visDays, getTwoWeekCost(lang, inten, isGroupClasses, visDays.Count), true, true));
                }
                else
                {
                    course = new KeyValuePair<Language, CourseInf>(lang, new CourseInf(inten, level, visDays, getTwoWeekCost(lang, inten, isGroupClasses, visDays.Count), false, true));
                }
            }
            else
            {
                course = new KeyValuePair<Language, CourseInf>(lang, new CourseInf(inten, level, visDays, getTwoWeekCost(lang, inten, isGroupClasses, visDays.Count), false, false));
            }

            return course;
        }

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

        public static Intensity generateIntensity(Level level)
        {
            int eval = rand.Next(100);

            switch (level)
            {
                case Level.LOW:
                    return eval < 70
                        ? Intensity.STANDARD
                        : Intensity.INTENSIVE;
                case Level.MIDDLE:
                    return eval < 10
                        ? Intensity.MAINTAINING
                        : eval < 80
                        ? Intensity.STANDARD
                        : Intensity.INTENSIVE;
                case Level.ADVANCED:
                    return eval < 10
                        ? Intensity.INTENSIVE
                        : eval < 60
                        ? Intensity.STANDARD
                        : Intensity.MAINTAINING;
                case Level.HIGH:
                    return eval < 5
                        ? Intensity.INTENSIVE
                        : eval < 30
                        ? Intensity.STANDARD
                        : Intensity.MAINTAINING;
                default: throw new ArgumentException(); 
            }
        }

        public static bool isGroup()
        {
            double rand = new Random().NextDouble();
            return rand > 0.15 ? true : false;
        }

        public static void addRandomStudent()
        {
            addStudent(GenerateStudent());
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
                case Language.GERMAN: cost = 16; break;
                case Language.FRENCH: cost = 17; break;
                case Language.SPANISH: cost = 18; break;
                case Language.RUSSIAN: cost = 19; break;
                case Language.JAPANESE: cost = 20; break;
                default: cost = 0; break;
            }
            if (intens == Intensity.INTENSIVE)
                cost *= 1.25;
            if (intens == Intensity.MAINTAINING)
                cost *= 0.75;
       
            cost *= 3;

            return (int) (cost * days);
        }

        private static LinkedList<DayOfWeek> generateVisitDaysOfWeek(bool isGroup)
        {
            LinkedList<DayOfWeek> days = new LinkedList<DayOfWeek>();
            Random rand = new Random();

            if (isGroup)
            {
                days = rand.NextDouble() > 0.4 ? getConWDays() : getEvenWDays();
            }
            else
            {
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (rand.NextDouble() < 0.6)
                    {
                        days.AddLast(day);
                    }
                }
                if (days.Count == 0)
                {
                    days.AddLast((DayOfWeek)rand.Next(6));
                }
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

        public static bool isVisitClass(int age)
        {
            return new Random().Next((int)((double)age / 10 + 7)) > 10;
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

        public static int getCourseDurationInWeeks(Intensity inten)
        {
            switch (inten)
            {
                case Intensity.INTENSIVE: return 4;
                case Intensity.STANDARD: return 8;
                case Intensity.MAINTAINING: return 12;
                default: return 0;
            }
        }


    }
}
