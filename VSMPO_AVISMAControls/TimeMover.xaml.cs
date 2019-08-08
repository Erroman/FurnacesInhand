using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VSMPO_AVISMAControls
{
    public class AlarmEventArgs : EventArgs
    {
        public int MillisecondsToAlarm
        {
            get;
            set;
        }
    }
partial class TimeMover : UserControl
    {
        private Clockwork clockwork;
        public  DependencyProperty DtValue;


        public DateTime Dt
        {
            get { return (DateTime)GetValue(DtProperty); }
            set { SetValue(DtProperty,value); }
        }

        // Using a DependencyProperty as the backing store for Dt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DtProperty =
            DependencyProperty.Register("Dt", typeof(DateTime), typeof(TimeMover),new FrameworkPropertyMetadata(new PropertyChangedCallback(OnDtChanged)));
        //update Dt in the clockwork by means of Metadata's method from here

        public TimeMover()
        {
            InitializeComponent();
            clockwork = (Clockwork)Resources["newClockwork"];
            clockwork.PropertyChanged += Clockwork_PropertyChanged;
            // this.DataContext = this.clockwork;
            // Binding b = new Binding(nameof(clockwork.Dt));
            // b.Source = clockwork;
            ////// b.Converter = new DTConverter();
            // b.Mode = BindingMode.TwoWay;
            // this.SetBinding(TimeMover.DtProperty,b);
            //this.millisecondBox.Text = "Куку";
            //this.secondBox.SetBinding(TextBox.TextProperty, b);
            //this.minuteBox.SetBinding(TextBox.TextProperty, b);
            //this.hourBox.SetBinding(TextBox.TextProperty, b);
            //this.datePicker.SetBinding(TextBox.TextProperty, b);

        }

        private void Clockwork_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName==nameof(clockwork.Dt))
            this.SetCurrentValue(DtProperty, clockwork.Dt);
        }
        private static void OnDtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           ((TimeMover)d).clockwork.Dt = (DateTime)e.NewValue;
        }
        //D:\Endeavours\WPF\MyBindingLab\MyBindingLab\TimeMover\TimeMover.xaml.cs

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
            if (clockwork.Milliseconds < 999)
            {
                clockwork.Milliseconds++;
            }
            else
                if (UpSecond())
                clockwork.Milliseconds = 0;
        }
        private void DownMillisecond()
        {
            if (clockwork.Milliseconds > 0)
            {
                clockwork.Milliseconds--;
            }
            else
                if(DownSecond())
                  clockwork.Milliseconds=999;
        }
        private bool UpSecond()
        {
            if (clockwork.Seconds < 59)
            {
                clockwork.Seconds++;
                return true;
            }
            else
                if (UpMinute())
                {
                   clockwork.Seconds = 0;
                   return true;
                }
               else
                   return false;
                
        }
        private bool DownSecond()
        {
            if (clockwork.Seconds > 0)
            {
                clockwork.Seconds--;
                return true;
            }
            else
                if(DownMinute())
                {
                  clockwork.Seconds = 59;
                  return true;
                }
                else
                   return false;
        }

        private bool UpMinute()
        {
            if (clockwork.Minutes < 59)
            {
                clockwork.Minutes++;
                return true;
            }
            else
                if(UpHour())
                {
                  clockwork.Minutes = 0;
                  return true;
                }
                else
                  return false;
        }
        private bool DownMinute()
        {
            if (clockwork.Minutes > 0)
            {
                clockwork.Minutes--;
                return true;
            }
            else
                if(DownHour())
                {
                  clockwork.Minutes = 59;
                  return true;
                }
                else
                  return false;
        }

       private bool UpHour()
        {
            if (clockwork.Hours < 23)
            {
                clockwork.Hours++;
                return true;
            }
            else
                if (UpDay())
            {
                clockwork.Hours = 0;
                return true;
            }
            else
                return false;
        }  
        private bool DownHour()
        {
            if (clockwork.Hours > 0)
            {
                clockwork.Hours--;
                return true;
            }
            else
                if(DownDay())
                {
                  clockwork.Hours = 23;
                  return true;
                }
                else
                  return false;
                 
               
        }

        private bool UpDay()
        {
            clockwork.Date = clockwork.Date.AddDays(1);
            return true;
        }
        private bool DownDay()
        {
            try
            {
                clockwork.Date = clockwork.Date.AddDays(-1);
                return true;
            }
            catch
            {
                return false;
            }
               
        }

    }

}