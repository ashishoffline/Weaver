# Weaver.Abstractions

High-performance, source-generated data access library for .NET.

## Features

- 🚀 **Compile-time code generation** - Zero reflection overhead
- 🔧 **Clean Architecture** - Interfaces and abstractions
- 🌟 **Modern C#** - Supports `init` properties and records
- ⚡ **Dapper-level performance** - With better memory usage

## Quick Start

```csharp
// 1. Mark your models
[GenerateMapper]
public class User
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

// 2. Use with dependency injection
services.AddScoped<IDbClient, WeaverDbClient>();

// 3. Query data
var users = await dbClient.QueryAsync<User>("SELECT * FROM Users");