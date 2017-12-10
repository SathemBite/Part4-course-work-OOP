using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    public class Student
    {
        public readonly string fullName;
        public readonly int age;
        public Dictionary<Language, CourseInf> courses { get; set; }



        public  Student(string fullName, int age, Dictionary<Language, CourseInf> courses)
        {
            this.fullName = fullName;
            this.age = age;
            this.courses = courses;
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

        public bool containsCourse(Level level)
        {
            return courses.Any(
                course => course.Value.level == level);
        }

        public bool containsCourse(Intensity inten)
        {
            return courses.Any(
                course => course.Value.inten == inten);
        }

        public bool containsCourse(DayOfWeek day, Intensity inten)
        {
            return courses.Any(
                course => course.Value.visitingDays.Contains(day) && course.Value.inten == inten);
        }


        public override string ToString()
        {
            return "Full name: " + fullName + "\nAge: " + age + "\nLanguage and level: ";
        }

        
    }
}
