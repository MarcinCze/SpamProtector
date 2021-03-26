using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib.Data
{
    public partial class RuleUsageStat
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int TimesUsed { get; set; }

        public virtual Rule Rule { get; set; }
    }
}
