using Claims.Application;
using Claims.Domain;

namespace Claims.Api.Dto;

public static class MappingExtensions
{
    public static CreateClaimRequest ToDomainRequest(this CreateClaimRequestDto source)
    {
        var claimTypeDto = source.Type;
        return new CreateClaimRequest(source.CoverId, source.Name, ToDomainEnum(claimTypeDto), source.DamageCost, source.Created);
    }

    public static ClaimDto? ToDto(this Claim? source)
    {
        return source != null
            ? new ClaimDto(source.Id, source.CoverId, source.Created, source.Name, ToDtoEnum(source.Type), source.DamageCost)
            : null;
    }

    public static ClaimType ToDomainEnum(this ClaimTypeDto source)
    {
        return source switch
        {
            ClaimTypeDto.Collision => ClaimType.Collision,
            ClaimTypeDto.Grounding => ClaimType.Grounding,
            ClaimTypeDto.BadWeather => ClaimType.BadWeather,
            ClaimTypeDto.Fire => ClaimType.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    public static ClaimTypeDto ToDtoEnum(this ClaimType source)
    {
        return source switch
        {
            ClaimType.Collision => ClaimTypeDto.Collision,
            ClaimType.Grounding => ClaimTypeDto.Grounding,
            ClaimType.BadWeather => ClaimTypeDto.BadWeather,
            ClaimType.Fire => ClaimTypeDto.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }
}
