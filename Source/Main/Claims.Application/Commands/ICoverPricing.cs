using Claims.Domain;

namespace Claims.Application.Commands;

public interface ICoverPricing
{
    decimal Premium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
