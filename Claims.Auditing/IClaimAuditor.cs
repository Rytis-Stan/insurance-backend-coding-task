namespace Claims.Auditing;

public interface IClaimAuditor
{
    void AuditClaimPost(Guid id);
    void AuditClaimDelete(Guid id);
}
