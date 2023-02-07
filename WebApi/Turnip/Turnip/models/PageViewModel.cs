using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turnip.Models
{
    public class PageViewModel : IPageViewModel, INotifyPropertyChanged
    {
        public LayoutModel Layout { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
        }
    }
}