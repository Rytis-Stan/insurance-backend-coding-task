namespace Auditing.Auditors.MessageQueueBased;

public record AuditMessage(Guid EntityId, HttpRequestType HttpRequestType);
