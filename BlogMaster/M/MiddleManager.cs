using BlogMaster.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlogMaster.M
{
    public class MiddleManager
    {
        private AsyncObservableCollection<LowestSlave> mSlavePool;
        private SqlLiteManager mDb;
        public MiddleManager(SqlLiteManager db) {
            
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

        public async Task<int> RollingStone(int timer) {
            while (true) {
                //WebClient webClient = new WebClient();
                //String page = await webClient.DownloadStringTaskAsync("http://www.google.com");
                try
                {
                    if (this.mSlavePool.Count > 0)
                    {
                        for (int i = 0; i < 3; i++) {
                            // 한 번에 3개 씩 전달
                            LowestSlave slave = this.mSlavePool.First<LowestSlave>();
                            Task<bool> nop = slave.Work();
                            this.mSlavePool.Remove(slave);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Modeless MessageBox 
                }
                finally {
                    await MiddleManager.Delay(timer);
                }

                
            }
        }

        public static Task Delay(int millis) {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            Timer timer = new Timer(_ => tcs.SetResult(null), null, millis, Timeout.Infinite);
            tcs.Task.ContinueWith(delegate { timer.Dispose(); });
            return tcs.Task;
        }

        public void Del() {

        }

    }
}
