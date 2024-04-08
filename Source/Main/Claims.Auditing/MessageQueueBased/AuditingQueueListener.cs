using BuildingBlocks.MessageQueues;

namespace Claims.Auditing.MessageQueueBased;

public class AuditingQueueListener : IQueueListener<AuditMessage>
{
    private readonly Dictionary<AuditEntityKind, IHttpRequestAuditor> _auditorsByAuditEntityKind;

    public AuditingQueueListener(Dictionary<AuditEntityKind, IHttpRequestAuditor> auditorsByAuditEntityKind)
    {
        _auditorsByAuditEntityKind = auditorsByAuditEntityKind;
    }

    public void OnMessageReceived(AuditMessage message)
    {
        var auditor = _auditorsByAuditEntityKind[message.EntityType];
        switch (message.HttpRequestType)
        {
            case HttpRequestType.Post:
                auditor.AuditPost(message.EntityId);
                break;
            case HttpRequestType.Delete:
                auditor.AuditDelete(message.EntityId);
                break;
            default:
                // TODO: Use a custom exception type?
                throw new ArgumentOutOfRangeException(nameof(message));
        }
    }
}
