using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.M
{
    public class NaverStatistics
    {
        public int no { get; set; }
        public String keyword { get; set; }
        public int monthlyPcCnt { get; set; }
        public int monthlyMobCnt { get; set; }
        public String CompIndex { get; set; }
        public int blogCount { get; set; }
        public int noNaverBlogCount { get; set; }
        public int associateKeywordCount { get; set; }
    }
}
