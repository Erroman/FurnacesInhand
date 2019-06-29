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
            //Units of time calculation
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
            get;
            set;
        }
        public DateTime Date
        {
            get;
            set;
        }
        public int Hours
        {
            get
            {
                return _hours;
            }
            set
            {
                if (value < 24 & value >= 0)
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
                if (value < 60 & value >= 0)
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
                if (value < 60 & value >= 0)
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

                _milliseconds = value;
                OnPropertyChanged();
                if (Alarm_On)
                        if (AlarmProcedure != null)
                            AlarmProcedure(new AlarmEventArgs() { MillisecondsToAlarm = _milliseconds });
            }
        }

    }
}
