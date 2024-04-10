using BuildingBlocks.MessageQueues;

namespace Auditing.Auditors.MessageQueueBased;

public class MessageQueueAuditor : IHttpRequestAuditor
{
    private readonly IConnectedSendingQueue<AuditMessage> _queue;

    public MessageQueueAuditor(IConnectedSendingQueue<AuditMessage> queue)
    {
        _queue = queue;
    }

    public void AuditPost(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Post);
    }

    public void AuditDelete(Guid entityId)
    {
        Audit(entityId, HttpRequestType.Delete);
    }

    private void Audit(Guid entityId, HttpRequestType httpRequestType)
    {
        _queue.Send(new AuditMessage(entityId, httpRequestType));
    }
}
