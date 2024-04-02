using Claims.Domain;

namespace Claims.Application.Repositories;

public interface IClaimsRepository : IRepository<NewClaimInfo, Claim>
{
}