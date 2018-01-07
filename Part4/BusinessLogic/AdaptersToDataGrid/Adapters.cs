using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Part4.BusinessLogic.AdaptersToDataGrid
{
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
}
