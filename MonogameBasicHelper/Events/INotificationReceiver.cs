namespace MonogameBasicHelper.Events
{
    public interface INotificationReceiver<T>
    {
        void Notify(object sender, T argument);
    }
}
