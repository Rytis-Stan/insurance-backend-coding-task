namespace Claims.Auditing;

public record AuditMessage(AuditEntityKind EntityType, Guid EntityId, HttpRequestType HttpRequestType);