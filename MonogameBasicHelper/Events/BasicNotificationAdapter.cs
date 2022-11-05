namespace MonogameBasicHelper.Events
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
