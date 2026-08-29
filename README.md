[![](https://img.shields.io/nuget/v/soenneker.utils.dtos.streetaddress.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.dtos.streetaddress/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.dtos.streetaddress/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.utils.dtos.streetaddress/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.utils.dtos.streetaddress.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.utils.dtos.streetaddress/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.utils.dtos.streetaddress/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.utils.dtos.streetaddress/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Utils.Dtos.StreetAddress
A utility library for StreetAddress related operations.

## Installation

```bash
dotnet add package Soenneker.Utils.Dtos.StreetAddress
```

## Quick start

```csharp
using Soenneker.Utils.Dtos.StreetAddress;
```

Call the static `StreetAddressUtil` methods directly; no dependency-injection registration is required.

## Common operations

- `Parse()` - Parses comma-separated or multiline input into `StreetAddress`; throws `FormatException` when unrecognized.
- `TryParse()` - Attempts comma-separated or multiline parsing; blank or unrecognized input returns `false` and a null result.
