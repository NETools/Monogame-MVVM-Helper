using MonogameBasicHelper.MVVM;
using System;

namespace MonogameBasicHelper.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class TrackBarElement : UiElement
    {
        public int TickFrequency { get; set; } = 10;
        public int Min { get; set; }
        public int Max { get; set; }
        public int Start { get; set; }
        internal IValueTextConverter ValueToTextConverter { get; set; }
        public Type ValueToTextConverterType { get; set; }
    }
}
