using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMPO_AVISMAControls
{
    class Clockwork : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime _dt;
        public DateTime Dt
        {
            get
            {
                return _dt;
            }
            set
            {
                _dt = value;
                OnPropertyChanged(nameof(Dt));
            }
        }
        private int _milliseconds;
        public int Milliseconds
        {
            get
            {
                return _milliseconds;
            }
            set
            {
                _milliseconds = value;
                OnPropertyChanged(nameof(Milliseconds));
            }
        }
        private int _seconds;
        public int Seconds
        {
            get
            {
                return _seconds;
            }
            set
            {
                _seconds = value;
                OnPropertyChanged(nameof(Seconds));
            }
        }
        private int _minutes;
        public int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                _minutes = value;
                OnPropertyChanged(nameof(Minutes));
            }
        }
        private int _hours;
        public int Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                _hours = value;
                OnPropertyChanged(nameof(Hours));
            }
        }
        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        private void OnPropertyChanged(string property_name)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(property_name));
        }
    }
}
