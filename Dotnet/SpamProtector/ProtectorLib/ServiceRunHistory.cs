using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib
{
    public partial class ServiceRunHistory
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public string Branch { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ExecutionTime { get; set; }
        public string Status { get; set; }
        public string Information { get; set; }
    }
}
