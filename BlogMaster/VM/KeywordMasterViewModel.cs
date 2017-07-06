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
        //public AsyncObservableCollection<LowestSlave> mWorkers;
        public KeywordMasterViewModel(ISqlLiteManager manager) {
            //this.mWorkers = new AsyncObservableCollection<LowestSlave>();
            this.mManager = new MiddleManager(manager);
            //Task<int> nop = this.mManager.RollingStone(3000);
            Timer timer = new Timer();
            timer.Interval = 3000;
            timer.Elapsed += mManager.timer_Elapsed;
            timer.Start();

            Timer delayTimer = new Timer();
            delayTimer.Interval = 5000;
            delayTimer.Elapsed += mManager.UpdateKeywordTable;
            delayTimer.Start();
            AddKeyword = new RelayCommand(AddKeywords);

        }

        public ICollectionView SlaveWorkerQueueView {
            get { return CollectionViewSource.GetDefaultView(mManager.mSlavePool); }
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
