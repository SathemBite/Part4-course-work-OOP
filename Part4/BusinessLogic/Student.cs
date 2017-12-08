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
        private Dictionary<Language, SkillProps> skills;
        private VisitingDays visitingDays;
        private ClassTime classTime; 


        public  Student(string fullName, int age, Dictionary<Language, SkillProps> skills, ClassTime classTime, VisitingDays visitingDays)
        {
            this.fullName = fullName;
            this.age = age;
            this.skills = skills;
            this.visitingDays = visitingDays;
            this.classTime = classTime;
        }

        public override string ToString()
        {
            return "Full name: " + fullName + "\nAge: " + age + "\nLanguage and level: ";
        }

        
    }
}
