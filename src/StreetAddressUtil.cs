using System;
using Soenneker.Extensions.String;

namespace Soenneker.Utils.Dtos.StreetAddress;

/// <summary>
/// A utility library for StreetAddress related operations
/// </summary>
public static class StreetAddressUtil
{
    public static Soenneker.Dtos.StreetAddress.StreetAddress Parse(string address)
    {
        if (TryParse(address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress))
            return streetAddress!;

        throw new FormatException("The address string is not in the expected format.");
    }

    public static bool TryParse(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        if (address.IsNullOrWhiteSpace())
            return false;

        bool isCommaSeparated = address.IndexOf(',') >= 0;

        return isCommaSeparated
            ? TryParseCommaSeparatedAddress(address, out streetAddress)
            : TryParseMultiLineAddress(address, out streetAddress);
    }

    private static bool TryParseCommaSeparatedAddress(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        Span<char> buffer = stackalloc char[address.Length];
        address.CopyTo(buffer);

        var partCount = 0;
        var parts = new string[8];
        var start = 0;

        for (var i = 0; i <= buffer.Length; i++)
        {
            if (i == buffer.Length || buffer[i] == ',')
            {
                if (partCount >= parts.Length)
                    break;

                int len = i - start;
                if (len > 0)
                {
                    parts[partCount++] = new string(buffer.Slice(start, len)).Trim();
                }

                start = i + 1;
            }
        }

        if (partCount < 4)
            return false;

        try
        {
            bool hasLine2 = partCount > 4;

            var idx = 0;
            string line1 = parts[idx++];
            string? line2 = hasLine2 ? parts[idx++] : null;
            string city = parts[idx++];
            string state = parts[idx++];
            string postalCode = parts[idx++];
            string? country = partCount > idx ? parts[idx++] : null;
            string? additionalInfo = partCount > idx ? string.Join(", ", parts, idx, partCount - idx) : null;

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
        catch
        {
            return false;
        }
    }

    private static bool TryParseMultiLineAddress(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        string[] lines = address.Split('\n', '\r');
        var count = 0;
        var parts = new string[6];

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (trimmed.Length == 0)
                continue;

            if (count >= parts.Length)
                break;

            parts[count++] = trimmed;
        }

        if (count < 3)
            return false;

        try
        {
            string line1 = parts[0];
            string? line2 = count > 3 ? parts[1] : null;
            string cityStatePostal = parts[line2 != null ? 2 : 1];
            string country = parts[line2 != null ? 3 : 2];

            if (!TryExtractCityStatePostal(cityStatePostal, out string city, out string state, out string postalCode))
                return false;

            streetAddress = new Soenneker.Dtos.StreetAddress.StreetAddress
            {
                Line1 = line1,
                Line2 = line2,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country
            };

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryExtractCityStatePostal(string input, out string city, out string state, out string postalCode)
    {
        city = "";
        state = "";
        postalCode = "";

        ReadOnlySpan<char> span = input.AsSpan().Trim();

        int lastSpace = span.LastIndexOf(' ');
        if (lastSpace == -1) return false;

        int secondLastSpace = span.Slice(0, lastSpace).LastIndexOf(' ');
        if (secondLastSpace == -1) return false;

        city = span.Slice(0, secondLastSpace).ToString().Trim();
        state = span.Slice(secondLastSpace, lastSpace - secondLastSpace).ToString().Trim();
        postalCode = span.Slice(lastSpace).ToString().Trim();

        // State must be 2 letters and postal code must be 5+ characters
        if (state.Length != 2 || postalCode.Length < 5)
            return false;

        return true;
    }
}
