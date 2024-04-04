using Claims.Auditing.MessageQueues;

namespace Claims.Auditing;

public class MessageQueueClaimAuditor : MessageQueueAuditor, IClaimAuditor
{
    public MessageQueueClaimAuditor(ISendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Claim)
    {
    }
}