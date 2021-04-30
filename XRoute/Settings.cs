namespace XRoute
{
    class Settings
    {
        public RouteSettings[] Routes { get; set; }

        public class RouteSettings
        {
            public string AeTitle { get; set; }

            public int Port { get; set; }

            public bool AuthorizeSources { get; set; }

            public SourceSettings[] Sources { get; set; }

            public DestinationSettings[] Destinations { get; set; }
        }

        public class SourceSettings
        {
            public string AeTitle { get; set; }

            public string Host { get; set; }
        }

        public class DestinationSettings
        {
            public string AeTitle { get; set; }

            public string Host { get; set; }

            public int Port { get; set; }
        }
    }
}
