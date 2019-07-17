using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FurnacesInHand
{
    class FurnacesInHandViewModel
    {
        private double _canvasX;
        private double _canvasY;
        public event PropertyChangedEventHandler PropertyChanged;
        public double CanvasX
        {
            get { return _canvasX; }
            set
            {
                if (value.Equals(_canvasX)) return;
                _canvasX = value;
                //Calculate the time from the X-coordinate
                OnPropertyChanged();
            }
        }

        public double CanvasY
        {
            get { return _canvasY; }
            set
            {
                if (value.Equals(_canvasY)) return;
                _canvasY = value;
                OnPropertyChanged();
            }
        }
        private DateTime _dtBegTime;
        private DateTime _dtEndTime;
        private DateTime _dtTimeMoment;
        public DateTime DtBegTime
        {
            get
            {
                return _dtBegTime;
            }
            set
            {
                _dtBegTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime DtEndTime
        {
            get
            {
                return _dtEndTime;
            }
            set
            {
                _dtEndTime = value;
                OnPropertyChanged();
            }
        }
        public DateTime DtTimeMoment
        {
            get
            {
                return _dtTimeMoment;
            }
            set
            {
                _dtTimeMoment = value;
                OnPropertyChanged();
            }
        }
        private double _canvasVoltageHeight;
        private double _canvasVoltageWidth;
        public double CanvasVoltageHeight { get; set; }
        public double CanvasVoltageWidth { get; set; }

        private string _VoltageMax;
        private string _VoltageMin;
        public string VoltageMax
        {
            get { return _VoltageMax; }
            set
            {
                if (value.Equals(_VoltageMax)) return;
                _VoltageMax = value;
                OnPropertyChanged();
            }
        }
        public string VoltageMin
        {
            get { return _VoltageMin; }
            set
            {
                if (value.Equals(_VoltageMin)) return;
                _VoltageMin = value;
                OnPropertyChanged();
            }

        }
        private double _canvasCurrentHeight;
        private double _canvasCurrentWidth;
        public double CanvasCurrentHeight { get; set; }
        public double CanvasCurrentWidth { get; set; }

        private string _CurrentMax;
        private string _CurrentMin;
        public string CurrentMax
        {
            get { return _CurrentMax; }
            set
            {
                if (value.Equals(_CurrentMax)) return;
                _CurrentMax = value;
                OnPropertyChanged();
            }
        }
        public string CurrentMin
        {
            get { return _CurrentMin; }
            set
            {
                if (value.Equals(_CurrentMin)) return;
                _CurrentMin = value;
                OnPropertyChanged();
            }

        }
        private double _canvasVacuumHeight;
        private double _canvasVacuumWidth;
        public double CanvasVacuumHeight { get; set; }
        public double CanvasVacuumWidth { get; set; }

        private string _VacuumMax;
        private string _VacuumMin;
        public string VacuumMax
        {
            get { return _VacuumMax; }
            set
            {
                if (value.Equals(_VacuumMax)) return;
                _VacuumMax = value;
                OnPropertyChanged();
            }
        }
        public string VacuumMin
        {
            get { return _VacuumMin; }
            set
            {
                if (value.Equals(_VacuumMin)) return;
                _VacuumMin = value;
                OnPropertyChanged();
            }

        }
        private double _canvasSolenoidUHeight;
        private double _canvasSolenoidUWidth;
        public double CanvasSolenoidUHeight { get; set; }
        public double CanvasSolenoidUWidth { get; set; }

        private string _SolenoidUMax;
        private string _SolenoidUMin;
        public string SolenoidUMax
        {
            get { return _SolenoidUMax; }
            set
            {
                if (value.Equals(_SolenoidUMax)) return;
                _SolenoidUMax = value;
                OnPropertyChanged();
            }
        }
        public string SolenoidUMin
        {
            get { return _SolenoidUMin; }
            set
            {
                if (value.Equals(_SolenoidUMin)) return;
                _SolenoidUMin = value;
                OnPropertyChanged();
            }

        }

        private double _canvasSolenoidIHeight;
        private double _canvasSolenoidIWidth;
        public double CanvasSolenoidIHeight { get; set; }
        public double CanvasSolenoidIWidth { get; set; }

        private string _SolenoidIMax;
        private string _SolenoidIMin;
        public string SolenoidIMax
        {
            get { return _SolenoidIMax; }
            set
            {
                if (value.Equals(_SolenoidIMax)) return;
                _SolenoidIMax = value;
                OnPropertyChanged();
            }
        }
        public string SolenoidIMin
        {
            get { return _SolenoidIMin; }
            set
            {
                if (value.Equals(_SolenoidIMin)) return;
                _SolenoidIMin = value;
                OnPropertyChanged();
            }

        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
