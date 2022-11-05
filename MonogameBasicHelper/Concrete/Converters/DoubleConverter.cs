using MonogameBasicHelper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelper.Concrete.Converters
{
    public class DoubleConverter : IConverter
    {
        public object Convert(object value)
        {
            if (double.TryParse(value.ToString(), out double result))
                return result;
            else return null;
        }

        public object ConvertBack(object value)
        {
            return (int)((double)value);
        }
    }
}
