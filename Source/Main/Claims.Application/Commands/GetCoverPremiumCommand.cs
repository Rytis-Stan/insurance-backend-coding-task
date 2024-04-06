using Claims.Domain;

namespace Claims.Application.Commands;

public record GetCoverPremiumRequest(DateOnly StartDate, DateOnly EndDate, CoverType CoverType);

public interface IGetCoverPremiumCommand : ICommand<GetCoverPremiumRequest, decimal>
{
}

public class GetCoverPremiumCommand : IGetCoverPremiumCommand
{
    private readonly ICoverPricing _coverPricing;

    public GetCoverPremiumCommand(ICoverPricing coverPricing)
    {
        _coverPricing = coverPricing;
    }

    public Task<decimal> ExecuteAsync(GetCoverPremiumRequest request)
    {
        return Task.FromResult(_coverPricing.CalculatePremium(request.StartDate, request.EndDate, request.CoverType));
    }
}
