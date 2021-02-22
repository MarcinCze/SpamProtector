using ProtectorLib.Models;

using System.Collections.Generic;

namespace ProtectorLib.Messaging
{
    public interface IMessagingService
    {
        void SendMessage(ServiceRunDTO serviceRunMessage);
        void SendMessage(EmailDTO emailMessage);
        void SendMessages(IEnumerable<EmailDTO> emailMessages);
        void SendMessage(UsedRuleDTO usedRuleMessage);
    }
}
