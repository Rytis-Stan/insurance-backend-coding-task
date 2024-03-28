namespace Claims.Auditing;

public interface IClaimAuditor
{
    void AuditClaim(Guid id, string httpRequestType);
}
