using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Events
{
    public interface INotificationReceiver<T>
    {
        void Notify(object sender, T argument);
    }
}
