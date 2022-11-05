using MonogameBasicHelper.MVVM;
using System;

namespace MonogameBasicHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UiElement : Attribute
    {
        public string GroupName { get; set; }
        public string Text { get; set; }
        public int Width { get; set; } = 250;
        internal IConverter Converter { get; set; }
        public Type ValueConverterType { get; set; }
    }
}
