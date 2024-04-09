﻿using Claims.Api.Contracts.Dto;
using Claims.Api.Contracts.Messages;
using Claims.Application.Commands.CreateClaim;
using Claims.Application.Commands.CreateCover;
using Claims.Application.Commands.GetAllClaims;
using Claims.Application.Commands.GetAllCovers;
using Claims.Application.Commands.GetClaimById;
using Claims.Application.Commands.GetCover;
using Claims.Application.Commands.GetCoverPremium;
using Claims.Domain;

namespace Claims.Api;

/// <summary>
/// An extension method class for mapping DTOs to domain objects and vice versa.
/// </summary>
public static class MappingExtensions
{
    public static CreateCoverArgs ToCommandArgs(this CreateCoverRequest source)
    {
        return new CreateCoverArgs(source.StartDate, source.EndDate, source.Type.ToDomainEnum());
    }

    public static CreateClaimArgs ToCommandArgs(this CreateClaimRequest source)
    {
        return new CreateClaimArgs(source.CoverId, source.Name, source.Type.ToDomainEnum(), source.DamageCost, source.Created);
    }

    public static CreateClaimResponse ToResponse(this CreateClaimResult source)
    {
        return new CreateClaimResponse(source.Claim.ToDto());
    }

    public static GetClaimResponse ToResponse(this GetClaimByIdResult source)
    {
        return new GetClaimResponse(source.Claim?.ToDto());
    }

    public static GetClaimsResponse ToResponse(this GetAllClaimsResult source)
    {
        return new GetClaimsResponse(source.Claims.Select(x => x.ToDto()));
    }

    public static CreateCoverResponse ToResponse(this CreateCoverResult source)
    {
        return new CreateCoverResponse(source.Cover.ToDto());
    }

    public static GetCoverResponse ToResponse(this GetCoverResult source)
    {
        return new GetCoverResponse(source.Cover?.ToDto());
    }

    public static GetCoversResponse ToResponse(this GetAllCoversResult source)
    {
        return new GetCoversResponse(source.Covers.Select(x => x.ToDto()));
    }

    public static GetCoverPremiumResponse ToResponse(this GetCoverPremiumResult source)
    {
        return new GetCoverPremiumResponse(source.Premium);
    }

    public static CoverDto ToDto(this Cover source)
    {
        return new CoverDto(source.Id, source.StartDate, source.EndDate, source.Type.ToDtoEnum(), source.Premium);
    }

    public static ClaimDto ToDto(this Claim source)
    {
        return new ClaimDto(source.Id, source.CoverId, source.Name, source.Type.ToDtoEnum(), source.DamageCost, source.Created);
    }

    public static CoverType ToDomainEnum(this CoverDtoType source)
    {
        return source switch
        {
            CoverDtoType.Yacht => CoverType.Yacht,
            CoverDtoType.PassengerShip => CoverType.PassengerShip,
            CoverDtoType.ContainerShip => CoverType.ContainerShip,
            CoverDtoType.BulkCarrier => CoverType.BulkCarrier,
            CoverDtoType.Tanker => CoverType.Tanker,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    public static CoverDtoType ToDtoEnum(this CoverType source)
    {
        return source switch
        {
            CoverType.Yacht => CoverDtoType.Yacht,
            CoverType.PassengerShip => CoverDtoType.PassengerShip,
            CoverType.ContainerShip => CoverDtoType.ContainerShip,
            CoverType.BulkCarrier => CoverDtoType.BulkCarrier,
            CoverType.Tanker => CoverDtoType.Tanker,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    public static ClaimType ToDomainEnum(this ClaimDtoType source)
    {
        return source switch
        {
            ClaimDtoType.Collision => ClaimType.Collision,
            ClaimDtoType.Grounding => ClaimType.Grounding,
            ClaimDtoType.BadWeather => ClaimType.BadWeather,
            ClaimDtoType.Fire => ClaimType.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }

    public static ClaimDtoType ToDtoEnum(this ClaimType source)
    {
        return source switch
        {
            ClaimType.Collision => ClaimDtoType.Collision,
            ClaimType.Grounding => ClaimDtoType.Grounding,
            ClaimType.BadWeather => ClaimDtoType.BadWeather,
            ClaimType.Fire => ClaimDtoType.Fire,
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }
}