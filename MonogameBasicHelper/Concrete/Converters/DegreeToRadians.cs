using MonogameBasicHelper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelper.Concrete.Converters
{
    public class DegreeToRadians : IConverter
    {
        public object Convert(object value)
        {
            return ((int)value) * Math.PI / 180.0;
        }

        public object ConvertBack(object value)
        {
            return (int)((((double)value) / Math.PI) * 180.0);
        }
    }
}
