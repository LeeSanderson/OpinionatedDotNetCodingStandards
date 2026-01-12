using Opinionated.DotNet.CodingStandards.Tests.Helpers;

namespace Opinionated.DotNet.CodingStandards.Tests;

[CollectionDefinition(nameof(PackageCollection))]
public class PackageCollection : ICollectionFixture<PackageFixture>
{
}