namespace ProtectorLib.Configuration
{
    public class MailboxesConfig
    {
        public MailboxConfig MainBox { get; set; }
        public MailboxConfig SpamBox { get; set; }
        public MailboxConfig AdsBox { get; set; }
        public MailboxConfig WorkBox { get; set; }
    }
}
