namespace SoftwareCatalog.SystemTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

        int a = 10, b = 20;

        int answer = a + b;

        Assert.Equal(31, answer);

    }
}