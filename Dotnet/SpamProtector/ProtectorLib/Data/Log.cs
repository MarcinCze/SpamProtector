using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib.Data
{
    public partial class Log
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public DateTime CreationTime { get; set; }
        public string ServiceName { get; set; }
        public string Branch { get; set; }
        public string ServiceVersion { get; set; }
        public string Function { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string AdditionalData { get; set; }
    }
}
