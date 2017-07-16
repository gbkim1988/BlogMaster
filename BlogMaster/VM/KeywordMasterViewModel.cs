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
        public ICommand DeleteWorker { get; set; }
        public ICommand DeleteAll { get; set; }
        public ICommand QueryTables { get; set; }
        public MiddleManager mManager;
        public static String NaverApiURL = "https://api.naver.com";
        public static String ACCESS = "010000000072b86885ad17db12fa363fdae020da53551f6b67e3e90ce2ce7f13c316c3dff0";
        public static String Secret = "AQAAAAByuGiFrRfbEvo2P9rgINpTCIkxi8H80T8N412ypolxAw==";
        public static String CsID = "984499";
        public ISqlLiteManager mDBConnection;
        public int mKeywordRecordCount;
        public Timer timer;
        public Timer delayTimer;
        public Timer collectStatsTimer;
        public Timer StatsTimer;

        public String ControlFindKeyword { get; set; }
        public String ControlFindMobCnt { get; set; }
        public String ControlFindPcCnt { get; set; }
        public String ControlFindBlogTotal { get; set; }
        public String ControlFindNonNavBlogCnt { get; set; }
        
        

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
            ControlFindKeyword = "";
            ControlFindMobCnt = "";
            ControlFindPcCnt = "";
            ControlFindBlogTotal = "";
            ControlFindNonNavBlogCnt = "";
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

            /*
            StatsTimer = new Timer();
            collectStatsTimer.Interval = 10000;
            collectStatsTimer.Elapsed += GetCollectionView;
            */
            collectStatsTimer.Start();

            AddKeyword = new RelayCommand(AddKeywords);
            KillSwitch = new RelayCommand(KillTimer);
            Restarter = new RelayCommand(RestartTimer);
            CreateCSV = new RelayCommand(CreateCsvAPI);
            DeleteWorker = new RelayCommand(DeleteWorkerPool);
            DeleteAll = new RelayCommand(DeleteAllRecords);
            QueryTables = new DelegateCommand(() => {
                Task.Run(() => QueryStatisticsTable());
            });
        }
        private async Task<int> QueryStatisticsTable()
        {
            int FindKeyword = -3;
            int FindMobCnt = -3;
            int FindPcCnt = -3;
            int FindBlogTotal = -3;
            int FindNonNavBlogCnt = -3;
            /*
                public String ControlFindKeyword { get; set; }
                public String ControlFindMobCnt { get; set; }
                public String ControlFindPcCnt { get; set; }
                public String ControlFindBlogTotal { get; set; }
                public String ControlFindNonNavBlogCnt { get; set; }
            */


            if (ControlFindMobCnt != "")
            {
                int.TryParse(ControlFindMobCnt, out FindMobCnt);
            }
            if (ControlFindPcCnt != "")
            {
                int.TryParse(ControlFindPcCnt, out FindPcCnt);
            }
            if (ControlFindBlogTotal != "")
            {
                int.TryParse(ControlFindBlogTotal, out FindBlogTotal);
            }
            if (ControlFindNonNavBlogCnt != "")
            {
                int.TryParse(ControlFindNonNavBlogCnt, out FindNonNavBlogCnt);
            }
            NaverStatisticsRecord.Clear();
            IList<NaverStatistics> tmp = this.mDBConnection.RetrieveStatisticsList(); ;
            if (ControlFindKeyword != "")
            {
                tmp = tmp.
                    Where(x => x.monthlyMobCnt >= FindMobCnt).
                    Where(x => x.monthlyPcCnt >= FindPcCnt).
                    Where(x => x.blogCount >= FindBlogTotal).
                    Where(x => x.noNaverBlogCount >= FindNonNavBlogCnt).
                    Where(x => x.keyword.Contains(ControlFindKeyword)).
                    ToList<NaverStatistics>();
            }
            else
            {
                tmp = tmp.
                    Where(x => x.monthlyMobCnt >= FindMobCnt).
                    Where(x => x.monthlyPcCnt >= FindPcCnt).
                    Where(x => x.blogCount >= FindBlogTotal).
                    Where(x => x.noNaverBlogCount >= FindNonNavBlogCnt).
                    ToList<NaverStatistics>();
            }
            
            foreach(var item in tmp)
            {
                NaverStatisticsRecord.Add(item);
                KeywordRecordCount = NaverStatisticsRecord.Count;
            }
            //NaverStatisticsRecord = new AsyncObservableCollection<NaverStatistics>(tmp);
            

            return 0;
        }
        private void DeleteAllRecords()
        {
            MessageBoxResult Response =  MessageBox.Show("정말로 다 삭제하시겠습니까? 최범영님?", "WARNING", MessageBoxButton.YesNoCancel);
            if (Response == MessageBoxResult.No || Response == MessageBoxResult.Cancel)
            {
                Messenger.Default.Send(new StatusBarMessage { Message = String.Format("DB 초기화 취소 수신 : DB 초기화를 취소하셨습니다. ") });
            }
            else {
                KillTimer();
                this.mManager.InitAllTables();
                Messenger.Default.Send(new StatusBarMessage { Message = String.Format("DB 초기화 수신 : DB 초기화를 완료하였습니다. ") });
                MessageBox.Show("DB 초기화 완료");
            }

        }
        private void DeleteWorkerPool()
        {
            KillTimer(); // 현재 작업 대기열 모두 중단 상태 조정
            this.mManager.DeleteWorkQueueFromDB();
            Messenger.Default.Send(new StatusBarMessage { Message = String.Format("대기 작업열 초기화 요청 수신 : 모든 대기 작업열을 초기화 합니다. ") });
            
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

        private void GetCollectionView()
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
            //StatsTimer.Stop();
            //this.NaverStats.Clear();
            this.mManager.mSlavePool.Clear();
            Messenger.Default.Send(new StatusBarMessage { Message = String.Format("작업 중지 요청 수신 : 모든 작업이 중단 예정입니다. ") });
        }

        private void RestartTimer()
        {
            timer.Start();
            delayTimer.Start();
            collectStatsTimer.Start();
            //StatsTimer.Start();
            Messenger.Default.Send(new StatusBarMessage { Message = String.Format("작업 재개 요청 수신 : 작업을 시작 합니다. ") });
        }
    }
}
