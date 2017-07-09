using BlogMaster.U;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.M
{
    public class CollectSlave : Notifier
    {
        public enum Status
        {
            Pending = 1,
            Processing = 2,
            Finish = 3,
            Error = -1,
        }

        public Stopwatch mElapsed;
        public Status mStat;
        public String mKeyword;
        public String mWorkerName;
        public int mStemFrom;
        public ISqlLiteManager mLiteManager;

        public CollectSlave(String keyword, ISqlLiteManager db, String Worker = "Collector")
        {
            Stat = Status.Pending;
            this.mElapsed = new Stopwatch();
            Keyword = keyword;
            this.mLiteManager = db;
            WorkerName = SyncHelperUtil.RandomNameGenerator(Worker);
        }

        public int StemFrom
        {
            get
            {
                return this.mStemFrom;
            }
            private set
            {
                this.mStemFrom = value;
                RaisePropertyChanged("StemFrom");
            }
        }

        public string Keyword
        {
            get
            {
                return this.mKeyword;
            }

            private set
            {
                this.mKeyword = value;
                RaisePropertyChanged("Keyword");
            }
        }

        public Status Stat
        {
            get
            {
                return this.mStat;
            }

            private set
            {
                this.mStat = value;
                RaisePropertyChanged("Stat");
            }
        }

        public string WorkerName
        {
            get
            {
                return this.mWorkerName;
            }

            private set
            {
                this.mWorkerName = value;
                RaisePropertyChanged("WorkerName");
            }
        }

        public async Task<bool> Work()
        {
            /*
                테스트의 목적은 Pending 상태에서, 
                프로세스 상태, 그리고 Finish 상태로 페이즈 전환이 
                정확히 View 에 반영되는가에 달려있다. 
             */

            Stat = Status.Processing;
            //await U.AsyncHelperUtil.Delay(5000);
            var result = FetchingNaverHelper.NaverFavoriteKeyword(this.mKeyword);
            StemFrom = result.Length;
            try
            {
                try
                {
                    this.mLiteManager.AddCollectedKeyword(this.Keyword, this.mStemFrom, 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                foreach (var elem in result)
                {
                    try
                    {
                        this.mLiteManager.AddPendingKeyword(elem, 0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
            catch (Exception e)
            {
                Stat = Status.Error;
            }
            finally
            {
                Stat = Status.Finish;
                this.mElapsed.Start();
            }
            return true;
        }
    }
}
