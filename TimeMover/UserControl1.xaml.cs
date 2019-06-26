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

   
}