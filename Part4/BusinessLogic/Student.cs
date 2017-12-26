using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
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
                int classesInTwoWeeks = course.Value.duration * course.Value.visitingDays.Count();
                bool active;
                do
                {
                    active = course.Value.oneDayStep(age);
                } while (active && --classesInTwoWeeks > 0);
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
