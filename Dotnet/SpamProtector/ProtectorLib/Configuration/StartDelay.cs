using System;

namespace ProtectorLib.Configuration
{
    public static class StartDelay
    {
        public static TimeSpan ScanService => new TimeSpan(0, 0, 10);
        public static TimeSpan CatalogService => new TimeSpan(0, 5, 0);
        public static TimeSpan DeleteService => new TimeSpan(0, 15, 0);
        public static TimeSpan MarkingService => new TimeSpan(0, 0, 30);
    }
}
