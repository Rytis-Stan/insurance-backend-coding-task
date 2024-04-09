namespace Shared.Auditing.MessageQueueBased;

public record AuditMessage(AuditEntityKind EntityType, Guid EntityId, HttpRequestType HttpRequestType);