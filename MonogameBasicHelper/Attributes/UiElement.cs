using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameBasicHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UiElement : Attribute
    {
        public string GroupName { get; set; }
        public string LabelText { get; set; }
    }
}
