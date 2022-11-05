namespace MonogameBasicHelper.ContainerService
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
                else return default;
            }
            internal set
            {
                _value = value;
            }
        }
    }
}
