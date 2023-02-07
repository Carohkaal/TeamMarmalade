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

        public void Notify(object source, string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(source,
                    new PropertyChangedEventArgs(propertyName));
        }

        public void Notify(object source, string[] propertyNames)
        {
            if (PropertyChanged == null) return;
            foreach (var property in propertyNames)
                PropertyChanged(source,
                    new PropertyChangedEventArgs(property));
        }
    }
}