using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PastPapers.Objects
{
    public class GraphData
    {
        public List<string> Dates;
        public List<int> NumberOfPapers;
        public List<double> Percentages;

        public GraphData(List<string> dates, List<int> numberOfPapers, List<double> percentages)
        {
            Dates = dates;
            NumberOfPapers = numberOfPapers;
            Percentages = percentages;
        }
    }
}
