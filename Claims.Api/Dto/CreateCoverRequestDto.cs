﻿namespace Claims.Api.Dto;

public record CreateCoverRequestDto(
    DateOnly StartDate,
    DateOnly EndDate,
    CoverType Type
) : ICreateCoverRequest;