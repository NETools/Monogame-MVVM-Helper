using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelper.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class ScrollBarElement : UiElement
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Start { get; set; }
    }
}
