namespace Claims.Api.Auditing;

public interface IClaimAuditor
{
    void AuditClaimPost(Guid id);
    void AuditClaimDelete(Guid id);
}
