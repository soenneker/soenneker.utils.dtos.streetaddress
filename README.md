[![](https://img.shields.io/nuget/v/soenneker.utils.dtos.streetaddress.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.dtos.streetaddress/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.dtos.streetaddress/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.dtos.streetaddress/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.dtos.streetaddress.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.dtos.streetaddress/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.dtos.streetaddress/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.dtos.streetaddress/actions/workflows/codeql.yml)

# Soenneker.Utils.Dtos.StreetAddress

Parses two common U.S. address text layouts into `Soenneker.Dtos.StreetAddress.StreetAddress`.

## Installation

```bash
dotnet add package Soenneker.Utils.Dtos.StreetAddress
```

## Comma-separated addresses

```csharp
StreetAddress address = StreetAddressUtil.Parse(
    "123 Main St, Apt 4B, Springfield, IL, 62704, USA, Near the park");
```

The recognized field order is:

```text
Line1, [Line2,] City, State, PostalCode, [Country,] [AdditionalInfo]
```

The parser identifies the state/postal pair as a two-character state followed by a postal value of at least five characters. Remaining comma-separated parts after country are joined into `AdditionalInfo`.

## Multiline addresses

```csharp
const string input = """
    123 Main St
    Apt 4B
    Springfield IL 62704
    USA
    """;

bool parsed = StreetAddressUtil.TryParse(input, out StreetAddress? address);
```

Three non-empty lines are interpreted as `Line1`, `City State PostalCode`, and `Country`. Four or more non-empty lines use the second line as `Line2`. In the combined city line, the final space-delimited value is the postal code and the preceding two-character value is the state; everything before those values is the city.

## Failure and validation

`TryParse()` returns `false` with a null result for blank or structurally unrecognized input. `Parse()` throws `FormatException` for the same cases.

This is a structural parser, not postal validation or normalization. It does not verify that a state code, postal code, country, or street exists. Validate externally when accepting addresses for shipping, billing, identity, or compliance decisions.
