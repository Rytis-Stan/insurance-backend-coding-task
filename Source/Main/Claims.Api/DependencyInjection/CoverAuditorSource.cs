using Auditing.Auditors;

namespace Claims.Api.DependencyInjection;

public class CoverAuditorSource : SourceOf<IHttpRequestAuditor>
{
}
