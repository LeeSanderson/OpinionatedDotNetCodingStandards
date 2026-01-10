namespace Opinionated.DotNet.CodingStandards.Tests;

public class UnitTest1
{
    private const int I = 1;

    [Fact]
    public void Test1()
    {
        var now = DateTime.Now;
        Assert.Equal(1, I);
    }
}