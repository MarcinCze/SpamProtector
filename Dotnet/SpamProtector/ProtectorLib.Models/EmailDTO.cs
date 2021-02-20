using System;

namespace ProtectorLib.Models
{
    public class EmailDTO
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
        public DateTime VersionUpdateTime { get; set; }
    }
}
