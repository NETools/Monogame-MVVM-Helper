using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelperDLL.ContainerService
{
    public class LateInit<T>  
    {
        private T _value;
        internal bool Activated { get; set; }
        
        public T Value
        {
            get
            {
                if (Activated)
                    return _value;
                else return default(T);
            }
            internal set
            {
                _value = value;
            }
        }
    }
}
