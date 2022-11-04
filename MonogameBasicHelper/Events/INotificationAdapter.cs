using MonogameBasicHelperDLL.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Events
{
    public interface INotificationAdapter : IAdapter
    {
        void Notify<S>(object sender, dynamic argument);
    }
}
