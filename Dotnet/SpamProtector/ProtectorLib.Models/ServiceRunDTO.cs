using ProtectorLib.Models.Enums;

using System;

namespace ProtectorLib.Models
{
    public class ServiceRunDTO
    {
        public string ServiceName { get; set; }
        public string Branch { get; set; }
        public string ServiceVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ExecutionTime { get; set; }
        public string Status { get; set; }
        public string Information { get; set; }

        public ServiceStatus ServiceStatus => (ServiceStatus)Enum.Parse(typeof(ServiceStatus), Status, true);
    }
}
