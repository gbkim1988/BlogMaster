﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.M
{
    public interface ISqlLiteManager
    {
        string info();
        DataTable RunInsertSQL(string Sql);
        void AddCollectedKeyword(String keyword, int Count, int Processed);
        void AddPendingKeyword(String keyword, int Processed);
        IList<String> RetrievePendingList();
        IList<String> RetrieveKewordList();
        void AddStatisticsKeyword(String keyword, int monthlyPcCnt, int monthlyMobCnt, String comp, int blogCount, int noNaverBlogCount, int associateKeywordCount);
        IList<NaverStatistics> RetrieveStatisticsList();
        void UpdateKeywordTable(IList<String> keywords);
        void UpdatePendingTable(IList<String> keywords);
        IList<String> RetrieveProcessedKewordList();
        void DeleteUnPorcessedRecords();
        void DeleteAllTables();
    }
}
