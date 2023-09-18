namespace Game.Input
{
    public partial class @Controls
    {
        public static Controls Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Controls();
                    _instance.Enable();
                }
                return _instance;
            }
        }

        private static @Controls _instance;
    }
}
