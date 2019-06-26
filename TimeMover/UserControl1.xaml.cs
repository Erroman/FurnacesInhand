using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeMover
{
    public class AlarmEventArgs : EventArgs
    {
        public int MillisecondsToAlarm
        {
            get;
            set;
        }
    }

    public class ClockWatch : INotifyPropertyChanged
    {
        public bool Alarm_On = true;
        public event Action<AlarmEventArgs> AlarmProcedure;

        public event PropertyChangedEventHandler PropertyChanged;
        private int _hours;
        private int _minutes;
        private int _seconds;
        private int _milliseconds;
        private bool externalCorrection = true;
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            //Tick calculation
            if (propertyName == "Milliseconds" & externalCorrection)
            {
                externalCorrection = false;
                Hours = TimeSpan.FromMilliseconds(_milliseconds).Hours;
                externalCorrection = false;
                Minutes = TimeSpan.FromMilliseconds(_milliseconds).Minutes;
                externalCorrection = false;
                Seconds = TimeSpan.FromMilliseconds(_milliseconds).Seconds;
            }
            else
            {
                if (externalCorrection)
                    Milliseconds = (int)(new TimeSpan(Hours, Minutes, Seconds)).TotalSeconds;
                externalCorrection = true;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
                //count Milliseconds!
                return _milliseconds;
            }
            set
            {
                if (value < 1000 & value >= 0)
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