using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PastPapers.Objects
{
    public class PastPaper
    {
        public int ID;
        public string subject;
        public string month;
        public string year;
        public int mark;
        public int maxMark;
        public double percentage;
        public string grade;
        public string dateCompleted;
        public string module;

        public PastPaper(int ID, string subject, string month, string year, int mark, int maxMark, double percentage, string grade, string dateCompleted, string module)
        {
            this.ID = ID;
            this.subject = subject;
            this.month = month;
            this.year = year;
            this.mark = mark;
            this.maxMark = maxMark;
            this.percentage = percentage;
            this.grade = grade;
            this.dateCompleted = dateCompleted;
            this.module = module;
        }
    }
}
