using BuildingBlocks.MessageQueues;

namespace Auditing.Auditors.MessageQueueBased;

public class MessageQueueCoverAuditor : MessageQueueAuditor, ICoverAuditor
{
    public MessageQueueCoverAuditor(IConnectedSendingQueue<AuditMessage> queue)
        : base(queue, AuditEntityKind.Cover)
    {
    }
}