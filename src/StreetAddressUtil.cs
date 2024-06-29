using System;
using System.Linq;

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

    /// <summary>
    /// Tries to parse a formatted address string into a StreetAddress object.
    /// </summary>
    /// <param name="address">The address string to parse.</param>
    /// <param name="streetAddress">When this method returns, contains the StreetAddress object, if the parsing succeeded, or null if the parsing failed.</param>
    /// <returns>true if the address string was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(string address, out Soenneker.Dtos.StreetAddress.StreetAddress? streetAddress)
    {
        streetAddress = null;

        if (string.IsNullOrWhiteSpace(address))
        {
            return false;
        }

        // Split the address into parts by comma
        string[] parts = address.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 4)
        {
            return false;
        }

        try
        {
            // Initialize address components
            string street1 = parts[0].Trim();
            string? street2 = parts.Length > 4 ? parts[1].Trim() : null;
            string city = parts[street2 != null ? 2 : 1].Trim();
            string state = parts[street2 != null ? 3 : 2].Trim();
            string postalCode = parts[street2 != null ? 4 : 3].Trim();
            string? country = parts.Length > (street2 != null ? 5 : 4) ? parts[street2 != null ? 5 : 4].Trim() : null;
            string? additionalInfo = parts.Length > (street2 != null ? 6 : 5) ? string.Join(", ", parts.Skip(street2 != null ? 6 : 5).Select(p => p.Trim())) : null;

            // Create the StreetAddress object
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
}
