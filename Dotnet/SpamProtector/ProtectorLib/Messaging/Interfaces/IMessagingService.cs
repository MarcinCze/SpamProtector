using ProtectorLib.Models;

namespace ProtectorLib.Messaging
{
    public interface IMessagingService
    {
        void SendMessage(ServiceRun serviceRunMessage);
        void SendMessage(Email emailMessage);
        void SendMessage(UsedRule usedRuleMessage);
    }
}
