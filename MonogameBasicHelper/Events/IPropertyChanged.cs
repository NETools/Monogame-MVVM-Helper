using System;

namespace MonogameBasicHelper.Events
{
    public interface IPropertyChanged
    {
        public event Action<string> PropertyChanged;
    }
}
