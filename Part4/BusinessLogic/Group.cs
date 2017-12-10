using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    class Group
    {
        private static int IDgener = 1;
        public int id { get; private set; }
        private LinkedList<Student> students;
        public Language lang { get; private set; }
        public Level level { get; private set; }
        public Intensity inten { get; private set; }
        public LinkedList<DayOfWeek> visDays;

        public Group(Language lang, Level level, Intensity inten, LinkedList<DayOfWeek> visDays)
        {
            this.lang = lang;
            this.level = level;
            this.inten = inten;
            this.visDays = visDays;
            id = IDgener++;
            students = new LinkedList<Student>();
        }



        private Student getStudent(int index)
        {
            return students.ElementAt(index);
        }

        private void removeStudent(int index)
        {
            students.Remove(getStudent(index));
        }

        public void addStudent(Student student)
        {
            students.AddLast(student);
        }

        public int getCountOfListeners()
        {
            return students.Count;
        }

        public static void moveStudents(Group src, Group dest, int count)
        {
            while (count-- > 0)
            {
                Student temp = src.getStudent(0);
                dest.addStudent(temp);
                src.removeStudent(0);
            }
        }

    }
}
