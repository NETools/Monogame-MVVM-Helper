using MonogameBasicHelper.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelper.Concrete.Converters
{
    public class DefaultValueToTextConverter : IValueTextConverter
    {
        public string Convert(object value)
        {
            return value.ToString();
        }
    }
}
