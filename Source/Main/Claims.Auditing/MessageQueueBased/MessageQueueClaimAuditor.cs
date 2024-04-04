using Claims.Auditing.MessageQueues;

namespace Claims.Auditing.MessageQueueBased;

public class MessageQueueClaimAuditor : MessageQueueAuditor, IClaimAuditor
{
    public MessageQueueClaimAuditor(ISendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Claim)
    {
    }
}