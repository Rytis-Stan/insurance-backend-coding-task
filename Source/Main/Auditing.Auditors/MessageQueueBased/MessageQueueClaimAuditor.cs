using BuildingBlocks.MessageQueues;

namespace Auditing.Auditors.MessageQueueBased;

public class MessageQueueClaimAuditor : MessageQueueAuditor, IClaimAuditor
{
    public MessageQueueClaimAuditor(IConnectedSendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Claim)
    {
    }
}