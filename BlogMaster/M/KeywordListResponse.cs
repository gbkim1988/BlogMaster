using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.M
{
    public class keyword
    {
        public String monthlyAvePcCtr { get; set; }
        public String monthlyMobileQcCnt { get; set; }
        public String monthlyAveMobileClkCnt { get; set; }
        public String plAvgDepth { get; set; }
        public String relKeyword { get; set; }
        public String monthlyPcQcCnt { get; set; }
        public String monthlyAveMobileCtr { get; set; }
        public String monthlyAvePcClkCnt { get; set; }
        public String compIdx { get; set; }

    }
    public class KeywordListResponse
    {
        public List<keyword> KeywordList { get; set; }
    }
}
