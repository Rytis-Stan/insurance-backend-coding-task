using BuildingBlocks.MessageQueues;

namespace Shared.Auditing.MessageQueueBased;

public class MessageQueueClaimAuditor : MessageQueueAuditor, IClaimAuditor
{
    public MessageQueueClaimAuditor(IConnectedSendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Claim)
    {
    }
}