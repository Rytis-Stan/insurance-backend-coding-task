﻿namespace Claims.Tests;

/// <summary>
/// A static utility class for creating various types of test data. The main concern of
/// the construction methods is to make them as short as possible, to help with the
/// readability of tests cases.
/// </summary>
public static class TestValueBuilder
{
    public static DateOnly Date(int year, int month, int day)
    {
        return new DateOnly(year, month, day);
    }
}
