using System;
using System.Windows;
using System.Windows.Controls;
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
partial class UserControl1 : UserControl
    { 
    public ClockWatch clockWatch;
    public UserControl1()
    {
        InitializeComponent();
        clockWatch = (ClockWatch)Resources["newClock"];
    }

        private void clockButton_Click(object O, RoutedEventArgs e)
        {
            Button someButton = O as Button;
            switch (someButton.Name)
            {
                case "hoursUp":
                    UpHour();
                    break;
                case "hoursDown":
                    DownHour();
                    break;
                case "minutesUp":
                    UpMinute();
                    break;
                case "minutesDown":
                    DownMinute();
                    break;
                case "secondsUp":
                    UpSecond();
                    break;
                case "secondsDown":
                    DownSecond();
                    break;
                case "MillisecondsUp":
                    UpTick();
                    break;
                case "MillisecondsDown":
                    DownTick();
                    break;
            }
        }
        private void DownSecond()
        {
            if (clockWatch.Seconds > 0)
                clockWatch.Seconds--;
            else
                if (DownMinute())
                clockWatch.Seconds = 59;
        }
        private void UpSecond()
        {
            if (clockWatch.Seconds < 59)
                clockWatch.Seconds++;
            else
                if (UpMinute())
                clockWatch.Seconds = 0;
        }

        private bool DownMinute()
        {
            if (clockWatch.Minutes > 0)
            {
                clockWatch.Minutes--;
                return true;
            }
            else

                if (DownHour())
            {
                clockWatch.Minutes = 59;
                return true;
            }
            else
                return false;
        }
        private bool UpMinute()
        {
            if (clockWatch.Minutes < 59)
            {
                clockWatch.Minutes++;
                return true;
            }
            else

                if (UpHour())
            {
                clockWatch.Minutes = 0;
                return true;
            }
            else
                return false;
        }

        private bool DownHour()
        {
            if (clockWatch.Hours > 0)
            {
                clockWatch.Hours--;
                return true;
            }
            else
                return false;
        }
        private bool UpHour()
        {
            if (clockWatch.Hours < 23)
            {
                clockWatch.Hours++;
                return true;
            }
            else
                return false;
        }
        private bool UpTick()
        {
            if (clockWatch.Milliseconds < 1000)
            {
                clockWatch.Milliseconds++;
                return true;
            }
            else
                return false;
        }
        private bool DownTick()
        {
            if (clockWatch.Milliseconds > 0)
            {
                clockWatch.Milliseconds--;
                return true;
            }
            else
                return false;
        }
    }

}