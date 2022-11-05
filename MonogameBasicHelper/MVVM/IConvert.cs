using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelper.MVVM
{
    public interface IConverter
    {
        object Convert(object value);
        object ConvertBack(object value);
    }
}
