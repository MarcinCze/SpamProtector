using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib.Data
{
    public partial class RuleType
    {
        public RuleType()
        {
            Rules = new HashSet<Rule>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Rule> Rules { get; set; }
    }
}
