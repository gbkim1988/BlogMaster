using BlogMaster.C;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlogMaster.VM
{
    public class MainViewModel : ViewModelBase
    {
        private String mlblStatusView;
        public String lblStatusView {
            get { return mlblStatusView; }
            set { mlblStatusView = value;
                RaisePropertyChanged("lblStatusView");
            }
        }
        public MainViewModel()
        {
            Messenger.Default.Register<StatusBarMessage>(this, ReceiveStatusMessage);
            
        }

        public void ReceiveStatusMessage(StatusBarMessage message) {
            this.lblStatusView = message.Message;
        }
    }
}
