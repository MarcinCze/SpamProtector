using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib
{
    public partial class ServiceRunSchedule
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Branch { get; set; }
        public DateTime? LastRun { get; set; }
        public int RunEveryDays { get; set; }
        public int RunEveryHours { get; set; }
        public int RunEveryMinutes { get; set; }
        public int RunEverySeconds { get; set; }
        public bool IsEnabled { get; set; }
    }
}
