using Soenneker.Tests.FixturedUnit;
using System;
using FluentAssertions;
using Xunit;

namespace Soenneker.Utils.Dtos.StreetAddress.Tests;

[Collection("Collection")]
public class StreetAddressUtilTests : FixturedUnitTest
{

    public StreetAddressUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Parse_ValidAddress_ReturnsStreetAddress()
    {
        // Arrange
        const string address = "123 Main St, Apt 4B, Springfield, IL, 62704, USA, Near the big park";

        // Act
        Soenneker.Dtos.StreetAddress.StreetAddress result = StreetAddressUtil.Parse(address);

        // Assert
        result.Line1.Should().Be("123 Main St");
        result.Line2.Should().Be("Apt 4B");
        result.City.Should().Be("Springfield");
        result.State.Should().Be("IL");
        result.PostalCode.Should().Be("62704");
        result.Country.Should().Be("USA");
        result.AdditionalInfo.Should().Be("Near the big park");
    }

    [Fact]
    public void Parse_USPS_address()
    {
        const string address = """
                               123 Street Address
                               Option Street 2
                               City ST 12345-9876
                               USA
                               """;

        Soenneker.Dtos.StreetAddress.StreetAddress result = StreetAddressUtil.Parse(address);

        result.Line1.Should().Be("123 Street Address");
        result.Line2.Should().Be("Option Street 2");
        result.City.Should().Be("City");
        result.State.Should().Be("ST");
        result.PostalCode.Should().Be("12345-9876");
        result.Country.Should().Be("USA");
    }

    [Fact]
    public void Parse_InvalidAddress_ThrowsFormatException()
    {
        // Arrange
        const string address = "Invalid Address";

        // Act
        Action act = () => StreetAddressUtil.Parse(address);

        // Assert
        act.Should().Throw<FormatException>().WithMessage("The address string is not in the expected format.");
    }

    [Fact]
    public void TryParse_ValidAddress_ReturnsTrueAndStreetAddress()
    {
        // Arrange
        var address = "123 Main St, Apt 4B, Springfield, IL, 62704, USA, Near the big park";

        // Act
        bool success = StreetAddressUtil.TryParse(address, out Soenneker.Dtos.StreetAddress.StreetAddress? result);

        // Assert
        success.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Line1.Should().Be("123 Main St");
        result.Line2.Should().Be("Apt 4B");
        result.City.Should().Be("Springfield");
        result.State.Should().Be("IL");
        result.PostalCode.Should().Be("62704");
        result.Country.Should().Be("USA");
        result.AdditionalInfo.Should().Be("Near the big park");
    }

    [Fact]
    public void TryParse_InvalidAddress_ReturnsFalse()
    {
        // Arrange
        const string address = "Invalid Address";

        // Act
        bool success = StreetAddressUtil.TryParse(address, out Soenneker.Dtos.StreetAddress.StreetAddress? result);

        // Assert
        success.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void TryParse_MissingOptionalFields_ReturnsTrueAndStreetAddress()
    {
        // Arrange
        const string address = "123 Main St, Springfield, IL, 62704";

        // Act
        bool success = StreetAddressUtil.TryParse(address, out Soenneker.Dtos.StreetAddress.StreetAddress? result);

        // Assert
        success.Should().BeTrue();
        result.Should().NotBeNull();
        result!.Line1.Should().Be("123 Main St");
        result.Line2.Should().BeNull();
        result.City.Should().Be("Springfield");
        result.State.Should().Be("IL");
        result.PostalCode.Should().Be("62704");
        result.Country.Should().BeNull();
        result.AdditionalInfo.Should().BeNull();
    }
}
