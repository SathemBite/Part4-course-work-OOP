using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    public class CourseInf
    {

        public Intensity inten;
        public Level level;
        public LinkedList<DayOfWeek> visitingDays;
        public LinkedList<bool> visits;
        public int costPerTwoW;
        public int duration;
        public bool isGroup;
        public CourseStatus status;
        public bool isWishGroup;




        public CourseInf(Intensity inten, Level level, LinkedList<DayOfWeek> visitingDays, int costPerTwoW, bool isGroup, bool isWishGroup)
        {
            this.inten = inten;
            this.level = level;
            this.visitingDays = visitingDays;
            this.costPerTwoW = costPerTwoW;
            this.visits = new LinkedList<bool>();
            this.status = CourseStatus.ACTIVE;
            this.isGroup = isGroup;
            this.duration = isWishGroup ? isGroup ? Courses.getCourseDurationInWeeks(inten) : 4 : Courses.getCourseDurationInWeeks(inten) * 2;
            this.isWishGroup = isWishGroup;
        }

        public bool oneDayStep(int age)
        {
            if (Courses.isLeaveCourse(visits.Count()))
            {
                status = CourseStatus.LEAVED;
                return false;
            }
            else
            {
                if (visits.Count() / visitingDays.Count() == duration)
                {
                    status = CourseStatus.FINISHED;
                    return false;
                }
                else
                {
                    visits.AddLast(Courses.isVisitClass(age));
                    return true;
                }

            }
        }
    }

    public class Student
    {
        private static int ider = 1;
        public string fullName { get; set; }
        public int age { get; set; }
        public Dictionary<Language, CourseInf> courses { get; set; }
        public int id;
        public int countOfCourses { get; set; }


        public Student(string fullName, int age, Dictionary<Language, CourseInf> courses)
        {
            this.fullName = fullName;
            this.age = age;
            this.courses = courses;
            this.id = ider++;
            this.countOfCourses = courses.Count;
        }

        public bool containsCourse(LinkedList<DayOfWeek> visDays, Language lang, Level level, Intensity inten)
        {

            return courses.Any(
                course =>
                course.Key == lang &&
                Enumerable.SequenceEqual(course.Value.visitingDays, visDays) && 
                course.Value.level == level && 
                course.Value.inten == inten);
        }

        public void makeTwoWeekStep()
        {
            foreach (KeyValuePair<Language, CourseInf> course in courses)
            {
                int classesInTwoWeeks = 2 * course.Value.visitingDays.Count();
                if (getStatus(course.Key) == CourseStatus.ACTIVE)
                {
                    do{} while (course.Value.oneDayStep(age) && --classesInTwoWeeks > 0);
                }
                
            }
        }

        public CourseStatus getStatus(Language lang)
        {
            CourseInf course;
            CourseStatus status;

            if (courses.TryGetValue(lang, out course))
            {
                return course.status;
            }

            return CourseStatus.LEAVED;
        }

        public bool containsCourse(Language lang, Level level)
        {
            return courses.Any(
                course => course.Value.level == level && course.Key == lang);
        }

        public bool containsCourse(Language lang, Intensity inten)
        {
            return courses.Any(
                course => course.Value.inten == inten && course.Key == lang);
        }

        public bool containsCourse(Language lang, DayOfWeek day, Intensity inten)
        {
            return courses.Any(
                course => course.Value.visitingDays.Contains(day) && course.Value.inten == inten && course.Key == lang);
        }

        public bool isGroup(Language lang)
        {
            return courses.Any(course => course.Key == lang && course.Value.isGroup);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Full name: {0}\nAge: {1}\nCourses:\n", fullName, age);
            foreach (KeyValuePair<Language, CourseInf> course in courses)
            {
                sb.AppendFormat("{0} course, {1} level, {2} intensity, cost per two weeks: {3}", 
                    course.Key, course.Value.level, course.Value.inten, course.Value.costPerTwoW);
            }
            return sb.ToString();
        }

        
    }
}
