using System;
using System.Collections.Generic;

#nullable disable

namespace ProtectorLib
{
    public partial class Message
    {
        public int Id { get; set; }
        public int ImapUid { get; set; }
        public string Mailbox { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public DateTime? CatalogTime { get; set; }
        public DateTime? PlannedRemoveTime { get; set; }
        public DateTime? RemoveTime { get; set; }
        public bool IsRemoved { get; set; }
    }
}
