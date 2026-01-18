using System;
using System.Diagnostics.Contracts;
using Soenneker.Extensions.Spans.Readonly.Chars;

namespace Soenneker.Utils.Dtos.StreetAddress;

/// <summary>
/// A utility library for StreetAddress related operations
/// </summary>
public static class StreetAddressUtil
{
    private const int MaxCommaParts = 8;
    private const int MaxLines = 6;

    [Pure]
    public static Soenneker.Dtos.StreetAddress.StreetAddress Parse(string address)
    {
        if (TryParse(address, out var streetAddress))
            return streetAddress!;

        throw new FormatException("The address string is not in the expected format.");
    }

    [Pure]
    public static bool TryParse(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        if (string.IsNullOrWhiteSpace(address))
            return false;

        // Avoid scanning twice unless needed
        ReadOnlySpan<char> span = address.AsSpan();
        return span.IndexOf(',') >= 0
            ? TryParseCommaSeparatedAddress(span, out streetAddress)
            : TryParseMultiLineAddress(span, out streetAddress);
    }

    private static bool TryParseCommaSeparatedAddress(ReadOnlySpan<char> address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        Span<Range> ranges = stackalloc Range[MaxCommaParts];
        int partCount = address.SplitCommaRanges(ranges);

        if (partCount < 4)
            return false;

        // Treat "Line2" as present when there are > 4 parts, matching your original logic.
        bool hasLine2 = partCount > 4;

        int idx = 0;

        ReadOnlySpan<char> line1Span = address[ranges[idx++]].Trim();
        ReadOnlySpan<char> line2Span = hasLine2 ? address[ranges[idx++]].Trim() : default;
        ReadOnlySpan<char> citySpan = address[ranges[idx++]].Trim();
        ReadOnlySpan<char> stateSpan = address[ranges[idx++]].Trim();
        ReadOnlySpan<char> postalSpan = idx < partCount ? address[ranges[idx++]].Trim() : default;

        if (line1Span.Length == 0 || citySpan.Length == 0 || stateSpan.Length == 0 || postalSpan.Length == 0)
            return false;

        string line1 = line1Span.ToString();
        string? line2 = hasLine2 && line2Span.Length != 0 ? line2Span.ToString() : null;
        string city = citySpan.ToString();
        string state = stateSpan.ToString();
        string postalCode = postalSpan.ToString();

        string? country = idx < partCount
            ? address[ranges[idx++]].TrimToNull()
            : null;

        string? additionalInfo = idx < partCount
            ? address.JoinCommaSeparated(ranges, idx, partCount - idx)
            : null;

        streetAddress = new Soenneker.Dtos.StreetAddress.StreetAddress
        {
            Line1 = line1,
            Line2 = line2,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            AdditionalInfo = additionalInfo
        };

        return true;
    }

    private static bool TryParseMultiLineAddress(ReadOnlySpan<char> address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        Span<Range> ranges = stackalloc Range[MaxLines];
        int count = address.SplitNonEmptyLineRanges(ranges);

        if (count < 3)
            return false;

        ReadOnlySpan<char> line1Span = address[ranges[0]].Trim();
        if (line1Span.Length == 0)
            return false;

        // Original behavior:
        // - If count > 3 => line2 = parts[1]
        // - Otherwise no line2
        bool hasLine2 = count > 3;

        ReadOnlySpan<char> line2Span = hasLine2 ? address[ranges[1]].Trim() : default;

        ReadOnlySpan<char> cityStatePostalSpan = address[ranges[hasLine2 ? 2 : 1]].Trim();
        ReadOnlySpan<char> countrySpan = address[ranges[hasLine2 ? 3 : 2]].Trim();

        if (cityStatePostalSpan.Length == 0 || countrySpan.Length == 0)
            return false;

        if (!TryExtractCityStatePostal(cityStatePostalSpan, out string city, out string state, out string postalCode))
            return false;

        streetAddress = new Soenneker.Dtos.StreetAddress.StreetAddress
        {
            Line1 = line1Span.ToString(),
            Line2 = hasLine2 && line2Span.Length != 0 ? line2Span.ToString() : null,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = countrySpan.ToString()
        };

        return true;
    }

    // Parses: "City ST 12345" where ST = 2 letters, postal len >= 5
    private static bool TryExtractCityStatePostal(ReadOnlySpan<char> span, out string city, out string state, out string postalCode)
    {
        city = "";
        state = "";
        postalCode = "";

        span = span.Trim();

        int lastSpace = span.LastIndexOf(' ');
        if (lastSpace <= 0 || lastSpace == span.Length - 1)
            return false;

        ReadOnlySpan<char> postalSpan = span.Slice(lastSpace + 1).Trim();
        ReadOnlySpan<char> beforePostal = span.Slice(0, lastSpace).Trim();

        int secondLastSpace = beforePostal.LastIndexOf(' ');
        if (secondLastSpace <= 0 || secondLastSpace == beforePostal.Length - 1)
            return false;

        ReadOnlySpan<char> stateSpan = beforePostal.Slice(secondLastSpace + 1).Trim();
        ReadOnlySpan<char> citySpan = beforePostal.Slice(0, secondLastSpace).Trim();

        if (stateSpan.Length != 2 || postalSpan.Length < 5 || citySpan.Length == 0)
            return false;

        // If you want to enforce letters-only state codes cheaply:
        // if (!char.IsLetter(stateSpan[0]) || !char.IsLetter(stateSpan[1])) return false;

        city = citySpan.ToString();
        state = stateSpan.ToString();
        postalCode = postalSpan.ToString();
        return true;
    }
}
