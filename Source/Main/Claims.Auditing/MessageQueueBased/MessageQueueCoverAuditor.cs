using BuildingBlocks.MessageQueues;

namespace Claims.Auditing.MessageQueueBased;

public class MessageQueueCoverAuditor : MessageQueueAuditor, ICoverAuditor
{
    public MessageQueueCoverAuditor(IConnectedSendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Cover)
    {
    }
}