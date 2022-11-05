using MonogameBasicHelper.MVVM;

namespace MonogameBasicHelper.Events
{
    public interface INotificationAdapter : IAdapter
    {
        void Notify<S>(object sender, dynamic argument);
    }
}
