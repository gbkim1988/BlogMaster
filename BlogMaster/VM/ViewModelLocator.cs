using BlogMaster.M;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlogMaster.VM
{
    public class ViewModelLocator
    {
        public ViewModelLocator() {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ISqlLiteManager, SqlLiteManager>();
            SimpleIoc.Default.Register(() => new KeywordMasterViewModel(new SqlLiteManager("blogmaster.sqlite")));
            Messenger.Default.Register<NotificationMessage>(this, NotifyUserMethod);
        }
        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public KeywordMasterViewModel KeywordMasterViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<KeywordMasterViewModel>();
            }
        }

        private void NotifyUserMethod(NotificationMessage message)
        {
            MessageBox.Show(message.Notification);
        }
    }
}
