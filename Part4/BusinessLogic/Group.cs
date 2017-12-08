using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    class Group
    {
        private static int IDgener = 0;
        private readonly int id;
        private LinkedList<Student> students;
        private readonly Language lang;
        private readonly Level level;
        private readonly Intensity inten;
        public int count;
        
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
