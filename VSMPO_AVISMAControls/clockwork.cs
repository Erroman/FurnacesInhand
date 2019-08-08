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
        private bool DtSet = false;
        private bool TimeSet = false;
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
                DtSet = true;
                if (!TimeSet)
                {
                    Milliseconds = Dt.Millisecond;
                    Seconds = Dt.Second;
                    Minutes = Dt.Minute;
                    Hours = Dt.Hour;
                    Date = Dt;
                }
                DtSet = false;
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
                TimeSet = true;
                if (!DtSet)
                {
                    Dt = new DateTime(Date.Year,Date.Month,Date.Day,Hours,Minutes,Seconds,Milliseconds);
                }
                TimeSet = false;
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
                TimeSet = true;
                if (!DtSet)
                {
                    Dt = new DateTime(Date.Year,Date.Month,Date.Day,Hours,Minutes,Seconds,Milliseconds);
                }
                TimeSet = false;
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
                TimeSet = true;
                if (!DtSet)
                {
                    Dt = new DateTime(Date.Year, Date.Month, Date.Day, Hours, Minutes, Seconds, Milliseconds);
                }
                TimeSet = false;
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
                TimeSet = true;
                if (!DtSet)
                {
                    Dt = new DateTime(Date.Year,Date.Month,Date.Day,Hours,Minutes,Seconds,Milliseconds);
                }
                TimeSet = false;
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
                TimeSet = true;
                if (!DtSet)
                {
                    Dt = new DateTime(Date.Year,Date.Month,Date.Day,Hours,Minutes,Seconds,Milliseconds);
                }
                TimeSet = false;
            }
        }
        private void OnPropertyChanged(string property_name)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(property_name));
        }
    }
}
