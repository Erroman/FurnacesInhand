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
