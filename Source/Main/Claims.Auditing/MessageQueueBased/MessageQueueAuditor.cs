using BuildingBlocks.MessageQueues;

namespace Claims.Auditing.MessageQueueBased;

public abstract class MessageQueueAuditor : IHttpRequestAuditor
{
    private readonly ISendingQueue<AuditMessage> _queue;
    private readonly AuditEntityKind _entityKind;

    protected MessageQueueAuditor(ISendingQueue<AuditMessage> queue, AuditEntityKind entityKind)
    {
        _queue = queue;
        _entityKind = entityKind;
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
        _queue.Send(new AuditMessage(_entityKind, entityId, httpRequestType));
    }
}