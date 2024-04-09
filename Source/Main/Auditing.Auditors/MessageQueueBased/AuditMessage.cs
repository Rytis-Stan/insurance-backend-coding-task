namespace Auditing.Auditors.MessageQueueBased;

public record AuditMessage(AuditEntityKind EntityType, Guid EntityId, HttpRequestType HttpRequestType);