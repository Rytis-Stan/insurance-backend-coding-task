using BuildingBlocks.MessageQueues;

namespace Auditing.Auditors.MessageQueueBased;

public class AuditingQueueListener : IQueueListener<AuditMessage>
{
    private readonly IHttpRequestAuditor _auditor;

    public AuditingQueueListener(IHttpRequestAuditor auditor)
    {
        _auditor = auditor;
    }

    public void OnMessageReceived(AuditMessage message)
    {
        switch (message.HttpRequestType)
        {
            case HttpRequestType.Post:
                _auditor.AuditPost(message.EntityId);
                break;
            case HttpRequestType.Delete:
                _auditor.AuditDelete(message.EntityId);
                break;
            default:
                // TODO: Use a custom exception type?
                throw new ArgumentOutOfRangeException(nameof(message));
        }
    }
}
