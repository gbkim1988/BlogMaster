using BlogMaster.Async;
using BlogMaster.C;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace BlogMaster.M
{
    public class MiddleManager
    {
        public AsyncObservableCollection<LowestSlave> mSlavePool;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private ISqlLiteManager mDb;
        public MiddleManager(ISqlLiteManager db) {
            
            this.mSlavePool = new AsyncObservableCollection<LowestSlave>();
            this.mDb = db;
            
        }

        public void Add(LowestSlave slave) {
            try
            {                
                this.mSlavePool.Add(slave);
            }
            catch (Exception e) {
                MessageBox.Show("MiddleManager > Add Error " + e.Message);
            }
        }

        public void AddKeyword(String keyword) {
            this.mSlavePool.Add(new LowestSlave(keyword, this.mDb));
        }

        public async void timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if (this.mSlavePool.Count > 0)
            {
                List<LowestSlave> delList = new List<LowestSlave>();
                IList<LowestSlave> tmpList = this.mSlavePool.ToList<LowestSlave>();
                foreach (var slave in tmpList)
                {
                    if (slave.Stat == LowestSlave.Status.Pending)
                    {
                        Messenger.Default.Send(new StatusBarMessage { Message = String.Format("{0}-{1} : 작업을 시작하였습니다. ", slave.mWorkerName, slave.mKeyword) });
                        Task<bool> nop = slave.Work();
                    } else if (slave.Stat == LowestSlave.Status.Finish)
                    {
                        if (slave.mElapsed.ElapsedMilliseconds > 3000)
                        {
                            // 종료 후 3초 뒤에 자동으로 소거
                            delList.Add(slave);
                            
                        }
                    }
                }

                foreach (var nonslave in delList) {
                    this.mSlavePool.Remove(nonslave);
                }

            }
        }

        public void UpdateKeywordTable(object sender, ElapsedEventArgs e) {
            var PendingList = this.mDb.RetrievePendingList();
            if (PendingList.Count > 0) {
                foreach(var pending in PendingList)
                    this.mSlavePool.Add(new LowestSlave(pending, this.mDb));
            }
        }
        SemaphoreSlim sem = new SemaphoreSlim(1);
        /**
         *  RollingStone 대신에 타이머를 적용하는 방향은 어떠한가?  
         *  -> 타이머 적용 
         */

        public async Task<int> RollingStone(int timer) {
            while (true) {
                //WebClient webClient = new WebClient();
                //String page = await webClient.DownloadStringTaskAsync("http://www.google.com");
                try
                {
                    Console.WriteLine(this.mSlavePool.Count);
                    if (this.mSlavePool.Count > 0)
                    {
                        foreach (var slave in this.mSlavePool) {
                            if (slave.Stat == LowestSlave.Status.Pending)
                            {
                                await U.AsyncHelperUtil.Delay(timer);
                                Task<bool> nop = slave.Work();
                            } else if (slave.Stat == LowestSlave.Status.Finish) {
                                if (slave.mElapsed.ElapsedMilliseconds > 3000) {
                                    // 종료 후 3초 뒤에 자동으로 소거
                                    mSlavePool.Remove(slave);
                                }
                            }
                        }
                        /*
                        for (int i = 0; i < 3; i++) {
                            // 한 번에 3개 씩 전달
                            LowestSlave slave = this.mSlavePool.First<LowestSlave>();
                            Task<bool> nop = slave.Work();
                            // this.mSlavePool.Remove(slave);
                        }*/
                    }
                }
                catch (Exception e)
                {
                    // Modeless MessageBox 
                    Console.WriteLine(String.Format("Error in RollingStone {0}", e.Message));
                }
                finally {
                    // 
                    //
                }

                
            }
        }

        public void Del() {

        }

    }
}
