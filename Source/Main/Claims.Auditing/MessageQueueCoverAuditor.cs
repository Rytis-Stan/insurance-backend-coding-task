using Claims.Auditing.MessageQueues;

namespace Claims.Auditing;

public class MessageQueueCoverAuditor : MessageQueueAuditor, ICoverAuditor
{
    public MessageQueueCoverAuditor(ISendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Cover)
    {
    }
}