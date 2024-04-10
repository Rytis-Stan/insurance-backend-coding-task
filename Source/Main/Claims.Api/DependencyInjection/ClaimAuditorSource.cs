using Auditing.Auditors;

namespace Claims.Api.DependencyInjection;

public class ClaimAuditorSource : SourceOf<IHttpRequestAuditor>
{
}
