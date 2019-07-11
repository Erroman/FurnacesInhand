using System.Windows.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace VSMPO_AVISMAControls
{
    class DTConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // throw new NotImplementedException();
            return 999;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return 995;
        }
    }
}
