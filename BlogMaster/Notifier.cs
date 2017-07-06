using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster
{
    [Serializable]
    public class Notifier : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, 
                    new PropertyChangedEventArgs(propertyName)
                );
            }
        }

        protected void SetProperty<T>(ref T item, T value,
            [CallerMemberName] string propertyName = null) {
            if (!EqualityComparer<T>.Default.Equals(item, value)) {
                item = value;
                RaisePropertyChanged(propertyName);
            }
        }
    }
}
