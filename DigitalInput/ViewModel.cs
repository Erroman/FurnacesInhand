using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DigitalInput
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double _digitalNumber;
        public double DigitalNumber 
        {
            get { return _digitalNumber; }
            set { _digitalNumber = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DigitalNumber))); }
        }
    }
}
