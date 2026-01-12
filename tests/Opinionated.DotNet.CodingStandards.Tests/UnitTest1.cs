namespace Opinionated.DotNet.CodingStandards.Tests;

public class UnitTest1
{
    private readonly int _i = 1;

    [Fact]
    public void Test1()
    {
        // Don't use DateTime.Now, use DateTime.UtcNow
        _ = DateTime.UtcNow;

        // Don't use StringComparison.InvariantCulture, use StringComparison.Ordinal
        var fileName = Guid.NewGuid() + ".txt";
        _ = fileName.IndexOf("test", StringComparison.Ordinal);


        // Don't use Enum.TryParse without ignoreCase parameter
        Assert.True(Enum.TryParse<StringComparison>("StringComparison.Ordinal", true, out _));

        Assert.Equal(1, this._i);
    }
}