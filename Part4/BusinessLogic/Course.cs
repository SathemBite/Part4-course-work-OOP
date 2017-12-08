using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4
{
    class Course
    {
        public readonly Language language;
        private LinkedList<LinkedList<Group>> morningGroups;
        private LinkedList<LinkedList<Group>> afternoonGroups;
        private LinkedList<LinkedList<Group>> eveningGroups;

        public void regroup(LinkedList<Group> groups)
        {
            groups.OrderBy(group => group.count);
            while (groups.Last().count - groups.First().count > 1)
            {
                Group.moveStudents(
                    groups.Last(),
                    groups.First(),
                    Math.Min(Math.Abs(groups.Last().count - 7), Math.Abs(groups.First().count - 7)));
                groups.OrderBy(group => group.count);
            }
        }

    }
}
