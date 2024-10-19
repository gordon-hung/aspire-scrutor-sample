# aspire-scrutor-sample
ASP.NET Core 8.0 Aspire Scrutor

> Scrutor - I search or examine thoroughly; I probe, investigate or scrutinize  
> From scrūta, as the original sense of the verb was to search through trash. - https://en.wiktionary.org/wiki/scrutor

Assembly scanning and decoration extensions for Microsoft.Extensions.DependencyInjection

## Installation

Install the [Scrutor NuGet Package](https://www.nuget.org/packages/Scrutor).

### Package Manager Console

```
Install-Package Scrutor
```

### .NET Core CLI

```
dotnet add package Scrutor
```

## Usage

The library adds two extension methods to `IServiceCollection`:

* `Scan` - This is the entry point to set up your assembly scanning.
* `Decorate` - This method is used to decorate already registered services.

See **Examples** below for usage examples.
