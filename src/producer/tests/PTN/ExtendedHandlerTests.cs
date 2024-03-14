namespace Producer.Tests.PTN;

public class ExtendedHandlerTests : PtnTestBase<ExtendedHandlers>
{
    [Theory]
    [InlineData("Have I Got News For You S53E02 EXTENDED 720p HDTV x264-QPEL", true)]
    [InlineData("Better.Call.Saul.S03E04.CONVERT.720p.WEB.h264-TBS", false)]
    public void ShouldDetectExtendedCorrectly(string releaseName, bool expectedExtended)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedExtended, result.Extended);
    }
}