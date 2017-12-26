using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4.BusinessLogic.AdaptersToDataGrid
{
    class Adapters
    {
        public static List<StudentInfrom> toAllowableForm(LinkedList<Student> students)
        {
            return students.Select(student => new StudentInfrom(student)).ToList();
        }

    }
    public struct GroupInform
    {
        public int id { set; get; }
        public string lang { set; get; }
        public string level { set; get; }
        public string inten { set; get; }
        public int countOfListeners { set; get; }
        public string visDays { set; get; }
        public string getVisDays()
        {
            return visDays;
        }
    }

    public struct CourseInform
    {
        public Language lang { get; set; }
        public Level level { get; set; }
        public Intensity inten { get; set; }
        public string type { get; set; }
        public string daysOfVisit { get; set; }
        public int cost { get; set; }
        public CourseStatus status { get; set; }

        public CourseInform(KeyValuePair<Language, CourseInf> course)
        {
            this.lang = course.Key;
            this.level = course.Value.level;
            this.inten = course.Value.inten;
            this.type = course.Value.isGroup ? "Group" : "Individual";
            this.daysOfVisit = String.Join(" ", course.Value.visitingDays);
            this.cost = course.Value.costPerTwoW;
            this.status = course.Value.status;
        }
    }

    public struct StudentInfrom
    {
        public string fullName { set; get; }
        public int countOfCourses { set; get; }
        public int age { set; get; }

        public StudentInfrom(Student student)
        {
            this.fullName = student.fullName;
            this.age = student.age;
            this.countOfCourses = student.courses.Count();
        }
    }
}
