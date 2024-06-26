namespace Claims.Domain;

public interface ICoverPricing
{
    decimal Premium(DateOnly startDate, DateOnly endDate, CoverType coverType);
}
