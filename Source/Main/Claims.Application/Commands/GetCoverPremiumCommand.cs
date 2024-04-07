using Claims.Domain;

namespace Claims.Application.Commands;

public record GetCoverPremiumRequest(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);

public record GetCoverPremiumResponse(decimal Premium);
//
// public interface IGetCoverPremiumCommand : ICommand<GetCoverPremiumRequest, GetCoverPremiumResponse>
// {
// }

public class GetCoverPremiumCommand : ICommand<GetCoverPremiumRequest, GetCoverPremiumResponse>
{
    private readonly ICoverPricing _coverPricing;

    public GetCoverPremiumCommand(ICoverPricing coverPricing)
    {
        _coverPricing = coverPricing;
    }

    public Task<GetCoverPremiumResponse> ExecuteAsync(GetCoverPremiumRequest request)
    {
        decimal premium = _coverPricing.Premium(request.StartDate, request.EndDate, request.CoverType);
        return Task.FromResult(new GetCoverPremiumResponse(premium));
    }
}
