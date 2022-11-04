using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.Events
{
    public class BasicNotificationAdapter : INotificationAdapter
    {
        public void Notify<S>(object sender, dynamic argument)
        {
            foreach (dynamic instance in FlexibleEventHandler.Default().GetRelevantInstances<S>())
                instance.Notify(sender, argument);
        }
    }
}
