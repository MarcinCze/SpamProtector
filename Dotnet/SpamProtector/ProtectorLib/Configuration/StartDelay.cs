using System;

namespace ProtectorLib.Configuration
{
    public static class StartDelay
    {
#if DEBUG
        public static TimeSpan ScanService => new TimeSpan(0, 0, 1);
        public static TimeSpan CatalogService => new TimeSpan(0, 0, 1);
        public static TimeSpan DeleteService => new TimeSpan(0, 0, 1);
        public static TimeSpan MarkingService => new TimeSpan(0, 0, 1);
#else
        public static TimeSpan ScanService => new TimeSpan(0, 0, 10);
        public static TimeSpan CatalogService => new TimeSpan(0, 7, 0);
        public static TimeSpan DeleteService => new TimeSpan(0, 4, 0);
        public static TimeSpan MarkingService => new TimeSpan(0, 0, 30);
#endif
    }
}
