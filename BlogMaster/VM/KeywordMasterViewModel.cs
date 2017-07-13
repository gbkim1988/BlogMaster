using BlogMaster.Async;
using BlogMaster.C;
using BlogMaster.M;
using CsvHelper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        public ICommand KillSwitch { get; set; }
        public ICommand Restarter { get; set; }
        public ICommand CreateCSV { get; set; }
        public MiddleManager mManager;
        public static String NaverApiURL = "https://api.naver.com";
        public static String ACCESS = "0100000000afb7de6201403a6bb53a1339a839e695f73d852d763cc371f4ca6c48a3b7ee9a";
        public static String Secret = "AQAAAAAl3cX78O8vHpex4GEsNOW9R0D5M23d+OUrsBCRDryoRQ==";
        public static String CsID = "1208129";
        public ISqlLiteManager mDBConnection;
        public int mKeywordRecordCount;
        public Timer timer;
        public Timer delayTimer;
        public Timer collectStatsTimer;
        public Timer StatsTimer;
        public int KeywordRecordCount {
            get { return mKeywordRecordCount; }
            set {
                mKeywordRecordCount = value;
                RaisePropertyChanged("KeywordRecordCount");
            }
        }

        public AsyncObservableCollection<NaverStatistics> NaverStats;
        public AsyncObservableCollection<NaverStatistics> NaverStatisticsRecord {
            get { return NaverStats; }
            set {
                NaverStats = value;
                RaisePropertyChanged("NaverStatisticsRecord");
            }
        }
        //public AsyncObservableCollection<LowestSlave> mWorkers;
        public KeywordMasterViewModel(ISqlLiteManager manager) {
            //this.mWorkers = new AsyncObservableCollection<LowestSlave>();
            this.mDBConnection = manager;
            this.mManager = new MiddleManager(manager);
            NaverStats = new AsyncObservableCollection<NaverStatistics>();
            //Task<int> nop = this.mManager.RollingStone(3000);
            timer = new Timer();
            timer.Interval = 3000;
            timer.Elapsed += mManager.timer_Elapsed;
            timer.Start();

            delayTimer = new Timer();
            delayTimer.Interval = 5000;
            delayTimer.Elapsed += mManager.UpdateKeywordTable;
            delayTimer.Start();

            collectStatsTimer = new Timer();
            collectStatsTimer.Interval = 7000;
            collectStatsTimer.Elapsed += mManager.UpdateCollectTable;
            collectStatsTimer.Start();

            StatsTimer = new Timer();
            collectStatsTimer.Interval = 10000;
            collectStatsTimer.Elapsed += GetCollectionView;
            
            collectStatsTimer.Start();

            AddKeyword = new RelayCommand(AddKeywords);
            KillSwitch = new RelayCommand(KillTimer);
            Restarter = new RelayCommand(RestartTimer);
            CreateCSV = new RelayCommand(CreateCsvAPI);

        }
        private System.Threading.ReaderWriterLockSlim _readerWriterLock = new System.Threading.ReaderWriterLockSlim();
        private void CreateCsvAPI() {
            try
            {
                _readerWriterLock.EnterReadLock();
                string CurrentDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                String CsvPath = Path.Combine(CurrentDesktopPath, "BY_" + DateTime.Now.ToString("yyyy_MM_dd_sss"));
                //using (TextWriter writer = File.CreateText(CsvPath+".csv")) {
                using (var writer = new StreamWriter(File.Open(CsvPath + ".csv", FileMode.CreateNew), Encoding.GetEncoding("UTF-8")))
                {
                    var csv = new CsvWriter(writer);
                    csv.Configuration.Encoding = Encoding.UTF8;
                    foreach (var item in NaverStats)
                    {
                        csv.WriteRecord(item);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally {
                _readerWriterLock.ExitReadLock();
            }
        }

        public ICollectionView SlaveWorkerQueueView {
            get { return CollectionViewSource.GetDefaultView(mManager.mSlavePool); }
        }

        public ICollectionView KeywordMasterRecordView
        {
            get {return CollectionViewSource.GetDefaultView(NaverStats); }
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
            KeywordRecordCount = NaverStats.Count;
            //this.NaverStats = new AsyncObservableCollection<NaverStatistics>(this.NaverStats.OrderBy(i => i.monthlyPcCnt));
            //this.NaverStats.OrderBy(x => x.monthlyPcCnt).OrderBy(x => x.monthlyMobCnt);
            //return this.NaverStats;
        }

        private void AddKeywords() {
            if (this.Keyword != null) {
                string[] newKeywords = this.Keyword.Split(' ');
                if (newKeywords.Count() > 1)
                {
                    foreach (var key in newKeywords)
                    {
                        this.mManager.AddKeyword(key);
                    }
                }
                this.mManager.AddKeyword(this.Keyword);
            }
        }

        private void KillTimer()
        {
            timer.Stop();
            delayTimer.Stop();
            collectStatsTimer.Stop();
            StatsTimer.Stop();
            this.NaverStats.Clear();
            this.mManager.mSlavePool.Clear();
            Messenger.Default.Send(new StatusBarMessage { Message = String.Format("작업 중지 요청 수신 : 모든 작업이 중단 예정입니다. ") });
        }

        private void RestartTimer()
        {
            timer.Start();
            delayTimer.Start();
            collectStatsTimer.Start();
            StatsTimer.Start();
            Messenger.Default.Send(new StatusBarMessage { Message = String.Format("작업 재개 요청 수신 : 작업을 시작 합니다. ") });
        }
    }
}
