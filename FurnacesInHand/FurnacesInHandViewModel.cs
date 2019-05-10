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


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
