using Claims.Domain;

namespace Claims.Application.Commands;

public record GetCoverPremiumRequest(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);

public class GetCoverPremiumCommand : ICommand<GetCoverPremiumRequest, decimal>
{
    private readonly ICoverPricing _coverPricing;

    public GetCoverPremiumCommand(ICoverPricing coverPricing)
    {
        _coverPricing = coverPricing;
    }

    public async Task<decimal> ExecuteAsync(GetCoverPremiumRequest request)
    {
        return _coverPricing.CalculatePremium(request.StartDate, request.EndDate, request.CoverType);
    }
}
