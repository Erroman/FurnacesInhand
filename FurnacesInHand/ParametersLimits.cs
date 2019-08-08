using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FurnacesInHand
{
    class ParametersLimits : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _VoltageMax;
        private string _VoltageMin;
        public string VoltageMax
        {
            get { return _VoltageMax; }
            set
            {
                _VoltageMax = value;
                PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(_VoltageMax)));
            }
        }
        public string VoltageMin
        {
            get { return _VoltageMin; }
            set
            {
                _VoltageMin = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_VoltageMin)));
            }
        }
    }
}
