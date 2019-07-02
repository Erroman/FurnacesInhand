using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeMover
{
    public class ClockWatch : INotifyPropertyChanged
    {
        public bool Alarm_On = true;
        public event Action<AlarmEventArgs> AlarmProcedure;

        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime _dt;
        private DateTime _date;
        private int _hours;
        private int _minutes;
        private int _seconds;
        private int _milliseconds;
        private bool externalCorrection = true;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            //if (propertyName == "Dt" & externalCorrection)
            //{
            //    externalCorrection = false;
            //    Hours = _dt.Hour;
            //    externalCorrection = false;
            //    Minutes = Dt.Minute;                    
            //    externalCorrection = false;
            //    Seconds = Dt.Second;
            //}
            //else
            //{
            //    if (externalCorrection)
            //        Dt = new DateTime(0,1,Hours, Minutes, Seconds,Milliseconds);
            //    externalCorrection = true;
            //}
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        public DateTime Dt
        {
            get
            {
                return _dt;
            }
            set
            {
                _dt = value;
                OnPropertyChanged();
            }
        }
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }
        public int Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                if (_hours!=value & value < 24 & value >= 0)
                {
                    _hours = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Minutes
        {
            get
            {
                return _minutes;
            }
            set
            {
                if (_minutes!=value & value < 60 & value >= 0)
                {
                    _minutes = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Seconds
        {
            get
            {
                return _seconds;
            }
            set
            {
                if (_seconds!=value & value < 60 & value >= 0)
                {
                    _seconds = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Milliseconds
        {
            get
            {
                return _milliseconds;
            }
            set
            {
                if(_milliseconds!=value & _milliseconds<1000 & _milliseconds>=0)
                {
                    _milliseconds = value;
                    OnPropertyChanged();
                    if (Alarm_On)
                        if (AlarmProcedure != null)
                            AlarmProcedure(new AlarmEventArgs() { MillisecondsToAlarm = _milliseconds });
                }

            }
        }

    }
}
