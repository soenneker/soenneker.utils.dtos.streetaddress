using Soenneker.Extensions.String;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Soenneker.Utils.Dtos.StreetAddress;

/// <summary>
/// A utility library for StreetAddress related operations
/// </summary>
public static class StreetAddressUtil
{
    /// <summary>
    /// Parses a formatted address string into a StreetAddress object.
    /// </summary>
    /// <param name="address">The address string to parse.</param>
    /// <returns>A StreetAddress object.</returns>
    /// <exception cref="FormatException">Thrown when the address string is not in the expected format.</exception>
    public static Soenneker.Dtos.StreetAddress.StreetAddress Parse(string address)
    {
        if (TryParse(address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress))
        {
            return streetAddress!;
        }

        throw new FormatException("The address string is not in the expected format.");
    }

    public static bool TryParse(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        if (address.IsNullOrWhiteSpace())
            return false;

        // Determine if the address is in comma-separated format or multi-line format
        bool isCommaSeparated = address.Contains(',');

        return isCommaSeparated
            ? TryParseCommaSeparatedAddress(address, out streetAddress)
            : TryParseMultiLineAddress(address, out streetAddress);
    }

    // Method for parsing comma-separated addresses
    private static bool TryParseCommaSeparatedAddress(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        // Split by commas for comma-separated addresses
        string[] parts = address.Split([','], StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim())
                                .ToArray();

        if (parts.Length < 4)
            return false; // Ensure we have at least street, city, state, and country

        try
        {
            // Extract components
            string street1 = parts[0].Trim();
            string? street2 = parts.Length > 4 ? parts[1].Trim() : null;
            string city = parts[street2 != null ? 2 : 1].Trim();
            string state = parts[street2 != null ? 3 : 2].Trim();
            string postalCode = parts[street2 != null ? 4 : 3].Trim();
            string? country = parts.Length > (street2 != null ? 5 : 4) ? parts[street2 != null ? 5 : 4].Trim() : null;
            string? additionalInfo = parts.Length > (street2 != null ? 6 : 5) ? string.Join(", ", parts.Skip(street2 != null ? 6 : 5)) : null;

            // Assign parsed values to the StreetAddress object
            streetAddress = new Soenneker.Dtos.StreetAddress.StreetAddress
            {
                Street1 = street1,
                Street2 = street2,
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

    // Method for parsing multi-line addresses
    private static bool TryParseMultiLineAddress(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        // Split by newlines for multi-line addresses
        string[] parts = address.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => p.Trim())
                                .ToArray();

        if (parts.Length < 3)
            return false; // Ensure we have at least street, city/state/postal, and country

        try
        {
            string street1 = parts[0].Trim();
            string? street2 = parts.Length > 3 ? parts[1].Trim() : null;
            string cityStatePostal = parts[street2 != null ? 2 : 1].Trim();

            // Use the helper method to extract city, state, and postal code
            if (!TryExtractCityStatePostal(cityStatePostal, out string city, out string state, out string postalCode))
                return false;

            string country = parts[street2 != null ? 3 : 2].Trim();

            // Assign parsed values to the StreetAddress object
            streetAddress = new Soenneker.Dtos.StreetAddress.StreetAddress
            {
                Street1 = street1,
                Street2 = street2,
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country,
                AdditionalInfo = null // Handle additional info separately if needed
            };

            return true;
        }
        catch
        {
            return false;
        }
    }

    // Helper method for extracting city, state, and postal code from a single line
    private static bool TryExtractCityStatePostal(string cityStatePostal, out string city, out string state, out string postalCode)
    {
        city = "";
        state = "";
        postalCode = "";

        // Use a regular expression to extract city, state, and postal code
        Match match = Regex.Match(cityStatePostal, @"^(.*)\s+([A-Za-z]{2})\s+(\d{5}(?:-\d{4})?)$");

        if (!match.Success)
            return false;

        city = match.Groups[1].Value.Trim();
        state = match.Groups[2].Value.Trim();
        postalCode = match.Groups[3].Value.Trim();

        return true;
    }
}
