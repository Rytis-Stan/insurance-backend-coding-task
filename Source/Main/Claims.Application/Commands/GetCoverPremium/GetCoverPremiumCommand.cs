using Claims.Domain;

namespace Claims.Application.Commands.GetCoverPremium;

public class GetCoverPremiumCommand : ICommand<GetCoverPremiumArgs, GetCoverPremiumResponse>
{
    private readonly ICoverPricing _coverPricing;

    public GetCoverPremiumCommand(ICoverPricing coverPricing)
    {
        _coverPricing = coverPricing;
    }

    public Task<GetCoverPremiumResponse> ExecuteAsync(GetCoverPremiumArgs args)
    {
        decimal premium = _coverPricing.Premium(args.StartDate, args.EndDate, args.CoverType);
        return Task.FromResult(new GetCoverPremiumResponse(premium));
    }
}
