﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    public class Group
    {
        private static int IDgener = 1;
        public int id { get; set; }
        public LinkedList<Student> students;
        public Language lang { get; set; }
        public Level level { get; set; }
        public Intensity inten { get; set; }
        public LinkedList<DayOfWeek> visDays;
        public int studCount { get; set; }
        public int stringDays { get; set; }
        public int duration { get; set; }
        public int studyWeekNum { get; set; }

        public string strVDays { get; set; }

        public Group(Language lang, Level level, Intensity inten, LinkedList<DayOfWeek> visDays, int duration)
        {
            this.lang = lang;
            this.level = level;
            this.inten = inten;
            this.visDays = visDays;
            studCount = 0;
            id = IDgener++;
            students = new LinkedList<Student>();
            strVDays = String.Join(" ", visDays.ToArray());
            studyWeekNum = 0;
            this.duration = duration;
        }

        public bool makeTwoWeekStep()
        {
            studyWeekNum += 2;
            foreach (Student student in students)
            {
                if (student.getStatus(lang) == CourseStatus.FINISHED)
                {
                    return false;
                }
                if (student.getStatus(lang) == CourseStatus.LEAVED)
                {
                    students.Remove(student);
                }
            }

            return true;
        }



        private Student getStudent(int index)
        {
            return students.ElementAt(index);
        }

        private void removeStudent(int id)
        {
            studCount--;
            students.Remove(students.First(stud => stud.id == id));
        }

        public void addStudent(Student student)
        {
            studCount++;
            CourseInf cInf;
            student.courses.TryGetValue(lang, out cInf);
            if (!cInf.isGroup)
            {
                cInf.costPerTwoW /= 3;
            }
            cInf.isGroup = true;;
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
