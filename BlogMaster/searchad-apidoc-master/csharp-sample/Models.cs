﻿using System;
using System.Collections.Generic;

namespace Naver.SearchAd
{
    public class keyword {
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
    public class KeywordListResponse {
        public List<keyword> KeywordList { get; set; }
    }
    class CustomerLink
    {
        public int ManagerEnable { get; set; }

        public long ManagerCustomerId { get; set; }

        public string ManagerLoginId { get; set; }

        public string ManagerCompanyName { get; set; }

        public int RoleId { get; set; }

        public long ClientCustomerId { get; set; }

        public string ClientLoginId { get; set; }
    }

    class Campaign
    {
        public string NccCampaignId { get; set; }

        public string CampaignTp { get; set; }

        public long CustomerId { get; set; }

        public string Name { get; set; }

        public UserLock UserLock { get; set; }

        public int DeliveryMethod { get; set; }

        public int UseDailyBudget { get; set; }

        public long DailyBudget { get; set; }

        public int UsePeriod { get; set; }

        public DateTime PeriodStartDt { get; set; }

        public DateTime PeriodEndDt { get; set; }
    }

    enum UserLock
    {
        ENABLED = 0,
        PAUSED = 1
    }

}
