using BlogMaster.Async;
using BlogMaster.M;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.VM
{
    public class KeywordMasterViewModel : ViewModelBase
    {
        private MiddleManager mManager;
        public AsyncObservableCollection<LowestSlave> mWorkers;
        public KeywordMasterViewModel() {
            this.mWorkers = new AsyncObservableCollection<LowestSlave>();
            this.mManager = new MiddleManager(new SqlLiteManager("db"));
            Task<int> nop = this.mManager.RollingStone(3000);
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");
            this.mManager.AddKeyword("fuck");

        }
    }
}
