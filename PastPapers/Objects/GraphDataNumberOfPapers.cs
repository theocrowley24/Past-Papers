using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PastPapers.Objects
{
    public class GraphDataNumberOfPapers
    {
        public List<string> Dates;
        public List<int> NumberOfPapers;

        public GraphDataNumberOfPapers(List<string> dates, List<int> numberOfPapers)
        {
            Dates = dates;
            NumberOfPapers = numberOfPapers;
        }
    }
}
