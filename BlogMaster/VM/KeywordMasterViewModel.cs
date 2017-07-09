using BlogMaster.Async;
using BlogMaster.M;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BlogMaster.VM
{
    public class KeywordMasterViewModel : ViewModelBase
    {
        public String Keyword { get; set; }
        public ICommand AddKeyword { get; set; }
        public MiddleManager mManager;
        public static String NaverApiURL = "https://api.naver.com";
        public static String ACCESS = "0100000000afb7de6201403a6bb53a1339a839e695f73d852d763cc371f4ca6c48a3b7ee9a";
        public static String Secret = "AQAAAAAl3cX78O8vHpex4GEsNOW9R0D5M23d+OUrsBCRDryoRQ==";
        public static String CsID = "1208129";
        public ISqlLiteManager mDBConnection;
        AsyncObservableCollection<NaverStatistics> NaverStats;
        //public AsyncObservableCollection<LowestSlave> mWorkers;
        public KeywordMasterViewModel(ISqlLiteManager manager) {
            //this.mWorkers = new AsyncObservableCollection<LowestSlave>();
            this.mDBConnection = manager;
            this.mManager = new MiddleManager(manager);
            NaverStats = new AsyncObservableCollection<NaverStatistics>();
            //Task<int> nop = this.mManager.RollingStone(3000);
            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.Elapsed += mManager.timer_Elapsed;
            timer.Start();

            Timer delayTimer = new Timer();
            delayTimer.Interval = 5000;
            delayTimer.Elapsed += mManager.UpdateKeywordTable;
            delayTimer.Start();

            Timer collectStatsTimer = new Timer();
            collectStatsTimer.Interval = 7000;
            collectStatsTimer.Elapsed += mManager.UpdateCollectTable;
            collectStatsTimer.Start();

            Timer StatsTimer = new Timer();
            collectStatsTimer.Interval = 10000;
            collectStatsTimer.Elapsed += GetCollectionView;
            collectStatsTimer.Start();

            AddKeyword = new RelayCommand(AddKeywords);

        }

        public ICollectionView SlaveWorkerQueueView {
            get { return CollectionViewSource.GetDefaultView(mManager.mSlavePool); }
        }

        public ICollectionView KeywordMasterRecordView
        {
            get {
                return CollectionViewSource.GetDefaultView(NaverStats); }
        }

        private void GetCollectionView(object sender, ElapsedEventArgs e)
        {

            IList<NaverStatistics> tmp = this.mDBConnection.RetrieveStatisticsList();
            foreach(var item in tmp)
            {
                if (this.NaverStats.Where<NaverStatistics>(c => c.keyword == item.keyword).Count<NaverStatistics>() == 0)
                {
                    this.NaverStats.Add(item);
                } 
            }
            //this.NaverStats = new AsyncObservableCollection<NaverStatistics>(this.NaverStats.OrderBy(i => i.monthlyPcCnt));
            //this.NaverStats.OrderBy(x => x.monthlyPcCnt).OrderBy(x => x.monthlyMobCnt);
            //return this.NaverStats;
        }

        private void AddKeywords() {
            string [] newKeywords = this.Keyword.Split(' ');

            foreach (var key in newKeywords) {
                this.mManager.AddKeyword(key);
            }
            this.mManager.AddKeyword(this.Keyword);
        }
    }
}
