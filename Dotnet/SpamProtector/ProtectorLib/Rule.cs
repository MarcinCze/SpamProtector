using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib
{
    public partial class Rule
    {
        public Rule()
        {
            RuleUsageStats = new HashSet<RuleUsageStat>();
        }

        public int Id { get; set; }
        public int RuleTypeId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }

        public virtual RuleType RuleType { get; set; }
        public virtual ICollection<RuleUsageStat> RuleUsageStats { get; set; }
    }
}
