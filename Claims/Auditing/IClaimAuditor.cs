namespace Claims.Auditing;

public interface IClaimAuditor
{
    void AuditClaim(string id, string httpRequestType);
}
