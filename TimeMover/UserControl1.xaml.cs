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
                    UpMillisecond();
                    break;
                case "MillisecondsDown":
                    DownMillisecond();
                    break;
            }
        }
        private void UpMillisecond()
        {
            if (clockWatch.Milliseconds < 999)
            {
                clockWatch.Milliseconds++;
            }
            else
                if (UpSecond())
                clockWatch.Milliseconds = 0;
        }
        private void DownMillisecond()
        {
            if (clockWatch.Milliseconds > 0)
            {
                clockWatch.Milliseconds--;
            }
            else
                if(DownSecond())
                  clockWatch.Milliseconds=999;
        }
        private bool UpSecond()
        {
            if (clockWatch.Seconds < 59)
            {
                clockWatch.Seconds++;
                return true;
            }
            else
                if (UpMinute())
                {
                   clockWatch.Seconds = 0;
                   return true;
                }
               else
                   return false;
                
        }
        private bool DownSecond()
        {
            if (clockWatch.Seconds > 0)
            {
                clockWatch.Seconds--;
                return true;
            }
            else
                if(DownMinute())
                {
                  clockWatch.Seconds = 59;
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
                if(UpHour())
                {
                  clockWatch.Minutes = 0;
                  return true;
                }
                else
                  return false;
        }
        private bool DownMinute()
        {
            if (clockWatch.Minutes > 0)
            {
                clockWatch.Minutes--;
                return true;
            }
            else
                if(DownHour())
                {
                  clockWatch.Minutes = 59;
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
                if (UpDay())
            {
                clockWatch.Hours = 0;
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
                if(DownDay())
                {
                  clockWatch.Hours = 23;
                  return true;
                }
                else
                  return false;
                 
               
        }

        private bool UpDay()
        {
            clockWatch.Date = clockWatch.Date.AddDays(1);
            return true;
        }
        private bool DownDay()
        {
            try
            {
                clockWatch.Date = clockWatch.Date.AddDays(-1);
                return true;
            }
            catch
            {
                return false;
            }
               
        }

    }

}